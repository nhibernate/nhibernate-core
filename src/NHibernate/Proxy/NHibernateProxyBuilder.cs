using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Proxy
{
	internal class NHibernateProxyBuilder
	{
		private const MethodAttributes constructorAttributes =
			MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;

		private static readonly System.Type NHibernateProxyType = typeof(INHibernateProxy);
		private static readonly PropertyInfo NHibernateProxyTypeLazyInitializerProperty = NHibernateProxyType.GetProperty(nameof(INHibernateProxy.HibernateLazyInitializer));
		private static readonly System.Type LazyInitializerType = typeof(ILazyInitializer);
		private static readonly PropertyInfo LazyInitializerIdentifierProperty = LazyInitializerType.GetProperty(nameof(ILazyInitializer.Identifier));
		private static readonly MethodInfo LazyInitializerInitializeMethod = LazyInitializerType.GetMethod(nameof(ILazyInitializer.Initialize));
		private static readonly MethodInfo LazyInitializerGetImplementationMethod = LazyInitializerType.GetMethod(nameof(ILazyInitializer.GetImplementation), System.Type.EmptyTypes);
		private static readonly PropertyInfo LazyInitializerIsUninitializedProperty = LazyInitializerType.GetProperty(nameof(ILazyInitializer.IsUninitialized));

		private readonly MethodInfo _getIdentifierMethod;
		private readonly MethodInfo _setIdentifierMethod;
		private readonly IAbstractComponentType _componentIdType;
		private readonly bool _interceptsEquals;

		public NHibernateProxyBuilder(MethodInfo getIdentifierMethod, MethodInfo setIdentifierMethod, IAbstractComponentType componentIdType, bool interceptsEquals)
		{
			_getIdentifierMethod = getIdentifierMethod;
			_setIdentifierMethod = setIdentifierMethod;
			_componentIdType = componentIdType;
			_interceptsEquals = interceptsEquals;
		}

		public TypeInfo CreateProxyType(System.Type baseType, IReadOnlyCollection<System.Type> baseInterfaces)
		{
			System.Type interfaceType = null;
			if (baseType == typeof(object))
			{
				// Mapping option "proxy" allows to ask for using an interface, which switches the base type to object
				// and adds the interface to base interfaces set.
				// Avoids using object for naming the proxy, as otherwise all entities using the "proxy" option for
				// specifying an interface would have their proxies sharing the same full name.
				interfaceType = baseInterfaces.FirstOrDefault(i => i != typeof(INHibernateProxy));
			}
			var typeName = $"{(interfaceType ?? baseType).Name}Proxy";
			var assemblyName = $"{typeName}Assembly";
			var moduleName = $"{typeName}Module";

			var name = new AssemblyName(assemblyName);

			var assemblyBuilder = ProxyBuilderHelper.DefineDynamicAssembly(AppDomain.CurrentDomain, name);
			var moduleBuilder = ProxyBuilderHelper.DefineDynamicModule(assemblyBuilder, moduleName);

			const TypeAttributes typeAttributes = TypeAttributes.AutoClass | TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.BeforeFieldInit;

			var interfaces = new HashSet<System.Type>
			{
				// Add the ISerializable interface so that it can be implemented
				typeof(ISerializable)
			};
			interfaces.UnionWith(baseInterfaces);
			interfaces.UnionWith(baseInterfaces.SelectMany(i => i.GetInterfaces()));
			interfaces.UnionWith(baseType.GetInterfaces());

			// Use the object as the base type 
			// since we're not inheriting from any class type
			var parentType = baseType;
			if (baseType.IsInterface)
			{
				parentType = typeof(object);
				interfaces.Add(baseType);
			}

#if NETFX || NETCOREAPP2_0
			var assemblyNamesToIgnoreAccessCheck =
				interfaces.Where(i => !i.IsVisible)
				          .Select(i => i.Assembly.GetName().Name)
				          .Distinct();
			foreach (var a in assemblyNamesToIgnoreAccessCheck)
				ProxyBuilderHelper.GenerateInstanceOfIgnoresAccessChecksToAttribute(assemblyBuilder, a);
#else
			interfaces.RemoveWhere(i => !i.IsVisible);
#endif
			
			var typeBuilder = moduleBuilder.DefineType(typeName, typeAttributes, parentType, interfaces.ToArray());

			var lazyInitializerField = typeBuilder.DefineField("__lazyInitializer", LazyInitializerType, FieldAttributes.Private);
			var proxyInfoField = typeBuilder.DefineField("__proxyInfo", typeof(NHibernateProxyFactoryInfo), FieldAttributes.Private);

			ImplementConstructor(typeBuilder, parentType, lazyInitializerField, proxyInfoField);

			// Provide a custom implementation of ISerializable instead of redirecting it back to the interceptor
			foreach (var method in ProxyBuilderHelper.GetProxiableMethods(baseType, interfaces.Except(new[] {typeof(ISerializable)})))
			{
				CreateProxiedMethod(typeBuilder, method, lazyInitializerField, parentType);
			}

			ProxyBuilderHelper.MakeProxySerializable(typeBuilder);
			ImplementDeserializationConstructor(typeBuilder, parentType);
			ImplementGetObjectData(typeBuilder, proxyInfoField, lazyInitializerField);

			var proxyType = typeBuilder.CreateTypeInfo();

			ProxyBuilderHelper.Save(assemblyBuilder);

			return proxyType;
		}

		private void CreateProxiedMethod(
			TypeBuilder typeBuilder, MethodInfo method, FieldInfo lazyInitializerField, System.Type parentType)
		{
			if (method == NHibernateProxyTypeLazyInitializerProperty.GetMethod)
			{
				ImplementGetLazyInitializer(typeBuilder, method, lazyInitializerField);
			}
			else if (method == _getIdentifierMethod)
			{
				ImplementGetIdentifier(typeBuilder, method, lazyInitializerField, parentType);
			}
			else if (method == _setIdentifierMethod)
			{
				ImplementSetIdentifier(typeBuilder, method, lazyInitializerField, parentType);
			}
			else if (!_interceptsEquals && method.Name == "Equals" && method.GetBaseDefinition() == typeof(object).GetMethod("Equals", new[] {typeof(object)}))
			{
				//skip
			}
			else if (!_interceptsEquals && method.Name == "GetHashCode" && method.GetBaseDefinition() == typeof(object).GetMethod("GetHashCode"))
			{
				//skip
			}
			else if (_componentIdType != null && _componentIdType.IsMethodOf(method))
			{
				ImplementCallMethodOnEmbeddedComponentId(typeBuilder, method, lazyInitializerField, parentType);
			}
			else
			{
				ImplementCallMethodOnImplementation(typeBuilder, method, lazyInitializerField, parentType);
			}
		}

		private static void ImplementConstructor(TypeBuilder typeBuilder, System.Type parentType, FieldInfo lazyInitializerField, FieldInfo proxyInfoField)
		{
			var constructor = typeBuilder.DefineConstructor(constructorAttributes, CallingConventions.Standard, new[] {LazyInitializerType, typeof(NHibernateProxyFactoryInfo)});

			var IL = constructor.GetILGenerator();

			constructor.SetImplementationFlags(MethodImplAttributes.IL | MethodImplAttributes.Managed);

			ProxyBuilderHelper.CallDefaultBaseConstructor(IL, parentType);

			// __lazyInitializer == lazyInitializer;
			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldarg_1);
			IL.Emit(OpCodes.Stfld, lazyInitializerField);

			// __proxyInfo == proxyInfo;
			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldarg_2);
			IL.Emit(OpCodes.Stfld, proxyInfoField);

			IL.Emit(OpCodes.Ret);
		}

		private static void ImplementDeserializationConstructor(TypeBuilder typeBuilder, System.Type parentType)
		{
			var parameterTypes = new[] {typeof (SerializationInfo), typeof (StreamingContext)};
			var constructor = typeBuilder.DefineConstructor(constructorAttributes, CallingConventions.Standard, parameterTypes);
			constructor.SetImplementationFlags(MethodImplAttributes.IL | MethodImplAttributes.Managed);

			var IL = constructor.GetILGenerator();
			ProxyBuilderHelper.CallDefaultBaseConstructor(IL, parentType);
			//Everything is done in NHibernateProxyObjectReference, so just return data.
			IL.Emit(OpCodes.Ret);
		}

		private static void ImplementGetObjectData(TypeBuilder typeBuilder, FieldInfo proxyInfoField, FieldInfo lazyInitializerField)
		{
			var methodBuilder = ProxyBuilderHelper.GetObjectDataMethodBuilder(typeBuilder);

			var IL = methodBuilder.GetILGenerator();

			// info.SetType(typeof(NHibernateProxyObjectReference));
			IL.Emit(OpCodes.Ldarg_1);
			IL.Emit(OpCodes.Ldtoken, typeof (NHibernateProxyObjectReference));
			IL.Emit(OpCodes.Call, ReflectionCache.TypeMethods.GetTypeFromHandle);
			IL.Emit(OpCodes.Callvirt, ProxyBuilderHelper.SerializationInfoSetTypeMethod);

			// return
			// 	(new NHibernateProxyObjectReference(
			// 		this.__proxyInfo,
			// 		this.__lazyInitializer.Identifier),
			// 		this.__lazyInitializer.IsUninitialized ? null : this.__lazyInitializer.GetImplementation())
			// 	.GetObjectData(info, context);
			//this.__proxyInfo
			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldfld, proxyInfoField);

			//this.__lazyInitializer.Identifier
			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldfld, lazyInitializerField);
			IL.Emit(OpCodes.Callvirt, LazyInitializerIdentifierProperty.GetMethod);

			// this.__lazyInitializer.IsUninitialized ? null : this.__lazyInitializer.GetImplementation()
			var isUnitialized = IL.DefineLabel();
			var endIsUnitializedTernary = IL.DefineLabel();
			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldfld, lazyInitializerField);
			IL.Emit(OpCodes.Callvirt, LazyInitializerIsUninitializedProperty.GetMethod);
			IL.Emit(OpCodes.Brtrue, isUnitialized);
			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldfld, lazyInitializerField);
			IL.Emit(OpCodes.Callvirt, LazyInitializerGetImplementationMethod);
			IL.Emit(OpCodes.Br, endIsUnitializedTernary);
			IL.MarkLabel(isUnitialized);
			IL.Emit(OpCodes.Ldnull);
			IL.MarkLabel(endIsUnitializedTernary);

			var constructor = typeof(NHibernateProxyObjectReference).GetConstructor(
				new[]
				{
					typeof(NHibernateProxyFactoryInfo),
					typeof(object),
					typeof(object)
				});
			IL.Emit(OpCodes.Newobj, constructor);

			IL.Emit(OpCodes.Ldarg_1);
			IL.Emit(OpCodes.Ldarg_2);
			IL.Emit(OpCodes.Callvirt, ProxyBuilderHelper.SerializableGetObjectDataMethod);

			IL.Emit(OpCodes.Ret);

			typeBuilder.DefineMethodOverride(methodBuilder, ProxyBuilderHelper.SerializableGetObjectDataMethod);
		}

		private static void ImplementGetLazyInitializer(TypeBuilder typeBuilder, MethodInfo method, FieldInfo lazyInitializerField)
		{
			// get { return this.__lazyInitializer; }

			const MethodAttributes attributes =
				MethodAttributes.Private | MethodAttributes.Final | MethodAttributes.HideBySig |
				MethodAttributes.SpecialName | MethodAttributes.NewSlot | MethodAttributes.Virtual;
			
			var getMethod = typeBuilder.DefineMethod($"{NHibernateProxyType.FullName}.get_{nameof(INHibernateProxy.HibernateLazyInitializer)}", attributes, CallingConventions.HasThis, LazyInitializerType, System.Type.EmptyTypes);
			getMethod.SetImplementationFlags(MethodImplAttributes.Managed | MethodImplAttributes.IL);

			var IL = getMethod.GetILGenerator();

			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldfld, lazyInitializerField);
			IL.Emit(OpCodes.Ret);

			typeBuilder.DefineMethodOverride(getMethod, method);
		}

		private static void ImplementGetIdentifier(
			TypeBuilder typeBuilder, MethodInfo method, FieldInfo lazyInitializerField, System.Type parentType)
		{
			/*
			get 
			{
				if (this.__lazyInitializer == null)
					return base.get_<Identifier>();
				return (<ReturnType>)this.__lazyInitializer.Identifier;
			}
			 */
			var methodOverride = ProxyBuilderHelper.GenerateMethodSignature(method.Name, method, typeBuilder);

			var IL = methodOverride.GetILGenerator();

			EmitCallBaseIfLazyInitializerIsNull(IL, method, lazyInitializerField, parentType);

			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldfld, lazyInitializerField);
			IL.Emit(OpCodes.Callvirt, LazyInitializerIdentifierProperty.GetMethod);
			IL.Emit(OpCodes.Unbox_Any, method.ReturnType);
			IL.Emit(OpCodes.Ret);

			typeBuilder.DefineMethodOverride(methodOverride, method);
		}

		private static void ImplementSetIdentifier(
			TypeBuilder typeBuilder, MethodInfo method, FieldInfo lazyInitializerField, System.Type parentType)
		{
			/*
			 set
			 {
				if (this.__lazyInitializer == null)
					return base.set_<Identifier>(value);
				this.__lazyInitializer.Initialize();
				this.__lazyInitializer.Identifier = value;
				this.__lazyInitializer.GetImplementation().<Identifier> = value;
			 }
			 */
			var propertyType = method.GetParameters()[0].ParameterType;
			var methodOverride = ProxyBuilderHelper.GenerateMethodSignature(method.Name, method, typeBuilder);
			var IL = methodOverride.GetILGenerator();

			EmitCallBaseIfLazyInitializerIsNull(IL, method, lazyInitializerField, parentType);

			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldfld, lazyInitializerField);
			IL.Emit(OpCodes.Callvirt, LazyInitializerInitializeMethod);

			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldfld, lazyInitializerField);
			IL.Emit(OpCodes.Ldarg_1);
			if (propertyType.IsValueType)
				IL.Emit(OpCodes.Box, propertyType);
			IL.Emit(OpCodes.Callvirt, LazyInitializerIdentifierProperty.SetMethod);

			EmitCallImplementation(IL, method, lazyInitializerField);

			IL.Emit(OpCodes.Ret);

			typeBuilder.DefineMethodOverride(methodOverride, method);
		}

		private static void ImplementCallMethodOnEmbeddedComponentId(
			TypeBuilder typeBuilder, MethodInfo method, FieldInfo lazyInitializerField, System.Type parentType)
		{
			/*
			 	if (this.__lazyInitializer == null)
					return base.<Method>(args..);
				this.__lazyInitializer.Identifier.<Method>(args..);
			*/
			var methodOverride = ProxyBuilderHelper.GenerateMethodSignature(method.Name, method, typeBuilder);

			var IL = methodOverride.GetILGenerator();

			EmitCallBaseIfLazyInitializerIsNull(IL, method, lazyInitializerField, parentType);

			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldfld, lazyInitializerField);
			IL.Emit(OpCodes.Callvirt, LazyInitializerIdentifierProperty.GetMethod);
			IL.Emit(OpCodes.Unbox_Any, method.DeclaringType);
			EmitCallMethod(IL, OpCodes.Callvirt, method);
			IL.Emit(OpCodes.Ret);

			typeBuilder.DefineMethodOverride(methodOverride, method);
		}

		private static void ImplementCallMethodOnImplementation(
			TypeBuilder typeBuilder, MethodInfo method, FieldInfo lazyInitializerField, System.Type parentType)
		{
			/*
				if (this.__lazyInitializer == null)
					return base.<Method>(args..);
				return this.__lazyInitializer.GetImplementation().<Method>(args..)
			 */
			var methodOverride = ProxyBuilderHelper.GenerateMethodSignature(method.Name, method, typeBuilder);

			var IL = methodOverride.GetILGenerator();

			EmitCallBaseIfLazyInitializerIsNull(IL, method, lazyInitializerField, parentType);

			EmitCallImplementation(IL, method, lazyInitializerField);
			IL.Emit(OpCodes.Ret);

			typeBuilder.DefineMethodOverride(methodOverride, method);
		}

		private static void EmitCallBaseIfLazyInitializerIsNull(
			ILGenerator IL, MethodInfo method, FieldInfo lazyInitializerField, System.Type parentType)
		{
			/*
				<if (method.DeclaringType.IsAssignableFrom(parentType))
				{>
				if (this.__lazyInitializer == null)
					return base.<method>(args..)
				<}>
			 */
			if (!method.DeclaringType.IsAssignableFrom(parentType))
				// The proxy does not derive from a type implementing the method, do not attempt
				// calling its base. In such case, the lazy initializer is never null.
				return;

			// When deriving from the entity class, the entity class constructor may trigger
			// virtual calls accessing the proxy state before its own constructor has a chance
			// to initialize it. So although lazyInitializer is never supplied as null to the
			// proxy constructor, we must guard nonetheless against it being null during base
			// constructor call.

			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldfld, lazyInitializerField);
			var skipBaseCall = IL.DefineLabel();

			IL.Emit(OpCodes.Ldnull);
			IL.Emit(OpCodes.Bne_Un, skipBaseCall);

			IL.Emit(OpCodes.Ldarg_0);
			EmitCallMethod(IL, OpCodes.Call, method);
			IL.Emit(OpCodes.Ret);

			IL.MarkLabel(skipBaseCall);
		}

		private static void EmitCallMethod(ILGenerator IL, OpCode opCode, MethodInfo method)
		{
			for (var i = 0; i < method.GetParameters().Length; i++)
				IL.Emit(OpCodes.Ldarg_S, (sbyte) (1 + i));
			IL.Emit(opCode, method);
		}

		private static void EmitCallImplementation(ILGenerator IL, MethodInfo method, FieldInfo lazyInitializerField)
		{
			//((<DeclaringType>)this.__lazyInitializer.GetImplementation()).<Method>(args..);
			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldfld, lazyInitializerField);
			IL.Emit(OpCodes.Callvirt, LazyInitializerGetImplementationMethod);
			IL.Emit(OpCodes.Unbox_Any, method.DeclaringType);
			EmitCallMethod(IL, OpCodes.Callvirt, method);
		}
	}
}
