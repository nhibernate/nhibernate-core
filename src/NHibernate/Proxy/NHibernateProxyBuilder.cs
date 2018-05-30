using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Security;
using NHibernate.Proxy.DynamicProxy;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Proxy
{
	internal class NHibernateProxyBuilder
	{
		private const MethodAttributes constructorAttributes =
			MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;

		private static readonly ConstructorInfo ObjectConstructor = typeof(object).GetConstructor(System.Type.EmptyTypes);

		private static readonly System.Type NHibernateProxyType = typeof(INHibernateProxy);
		private static readonly PropertyInfo NHibernateProxyTypeLazyInitializerProperty = NHibernateProxyType.GetProperty(nameof(INHibernateProxy.HibernateLazyInitializer));
		private static readonly System.Type LazyInitializerType = typeof(ILazyInitializer);
		private static readonly PropertyInfo LazyInitializerIdentifierProperty = LazyInitializerType.GetProperty(nameof(ILazyInitializer.Identifier));
		private static readonly MethodInfo LazyInitializerInitializeMethod = LazyInitializerType.GetMethod(nameof(ILazyInitializer.Initialize));
		private static readonly MethodInfo LazyInitializerGetImplementationMethod = LazyInitializerType.GetMethod(nameof(ILazyInitializer.GetImplementation), System.Type.EmptyTypes);
		private static readonly PropertyInfo LazyInitializerIsUninitializedProperty = LazyInitializerType.GetProperty(nameof(ILazyInitializer.IsUninitialized));
		private static readonly IProxyAssemblyBuilder ProxyAssemblyBuilder = new DefaultProxyAssemblyBuilder();

		private static readonly ConstructorInfo SecurityCriticalAttributeConstructor = typeof(SecurityCriticalAttribute).GetConstructor(System.Type.EmptyTypes);
		private static readonly MethodInfo SerializableGetObjectDataMethod = typeof(ISerializable).GetMethod(nameof(ISerializable.GetObjectData));
		private static readonly MethodInfo SerializationInfoSetTypeMethod = ReflectHelper.GetMethod<SerializationInfo>(si => si.SetType(null));
		
		private readonly MethodInfo _getIdentifierMethod;
		private readonly MethodInfo _setIdentifierMethod;
		private readonly IAbstractComponentType _componentIdType;
		private readonly bool _overridesEquals;
		
		public NHibernateProxyBuilder(MethodInfo getIdentifierMethod, MethodInfo setIdentifierMethod, IAbstractComponentType componentIdType, bool overridesEquals)
		{
			_getIdentifierMethod = getIdentifierMethod;
			_setIdentifierMethod = setIdentifierMethod;
			_componentIdType = componentIdType;
			_overridesEquals = overridesEquals;
		}

		public TypeInfo CreateProxyType(System.Type baseType, IReadOnlyCollection<System.Type> baseInterfaces)
		{
			var typeName = $"{baseType.Name}Proxy";
			var assemblyName = $"{typeName}Assembly";
			var moduleName = $"{typeName}Module";

			var name = new AssemblyName(assemblyName);

			var assemblyBuilder = ProxyAssemblyBuilder.DefineDynamicAssembly(AppDomain.CurrentDomain, name);
			var moduleBuilder = ProxyAssemblyBuilder.DefineDynamicModule(assemblyBuilder, moduleName);

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

			interfaces.RemoveWhere(i => !i.IsVisible);

			var typeBuilder = moduleBuilder.DefineType(typeName, typeAttributes, parentType, interfaces.ToArray());

			var lazyInitializerField = typeBuilder.DefineField("__lazyInitializer", LazyInitializerType, FieldAttributes.Private);
			var proxyInfoField = typeBuilder.DefineField("__proxyInfo", typeof(NHibernateProxyFactoryInfo), FieldAttributes.Private);

			ImplementConstructor(typeBuilder, parentType, lazyInitializerField, proxyInfoField);

			// Provide a custom implementation of ISerializable
			// instead of redirecting it back to the interceptor
			foreach (var method in ProxyFactory.GetProxiableMethods(baseType, interfaces.Except(new[] {typeof(ISerializable)})))
			{
				CreateProxiedMethod(typeBuilder, method, lazyInitializerField);
			}

			// Make the proxy serializable
			var serializableConstructor = typeof(SerializableAttribute).GetConstructor(System.Type.EmptyTypes);
			var customAttributeBuilder = new CustomAttributeBuilder(serializableConstructor, Array.Empty<object>());
			typeBuilder.SetCustomAttribute(customAttributeBuilder);

			ImplementDeserializationConstructor(typeBuilder, parentType);
			ImplementGetObjectData(typeBuilder, proxyInfoField, lazyInitializerField);

			var proxyType = typeBuilder.CreateTypeInfo();

			ProxyAssemblyBuilder.Save(assemblyBuilder);

			return proxyType;
		}

		private void CreateProxiedMethod(TypeBuilder typeBuilder, MethodInfo method, FieldInfo lazyInitializerField)
		{
			if (method == NHibernateProxyTypeLazyInitializerProperty.GetMethod)
			{
				ImplementGetLazyInitializer(typeBuilder, method, lazyInitializerField);
			}
			else if (method == _getIdentifierMethod)
			{
				ImplementGetIdentifier(typeBuilder, method, lazyInitializerField);
			}
			else if (method == _setIdentifierMethod)
			{
				ImplementSetIdentifier(typeBuilder, method, lazyInitializerField);
			}
			else if (!_overridesEquals && method.Name == "Equals" && method.GetBaseDefinition() == typeof(object).GetMethod("Equals", new[] {typeof(object)}))
			{
				//skip
			}
			else if (!_overridesEquals && method.Name == "GetHashCode" && method.GetBaseDefinition() == typeof(object).GetMethod("GetHashCode"))
			{
				//skip
			}
			else if (_componentIdType != null && _componentIdType.IsMethodOf(method))
			{
				ImplementCallMethodOnEmbeddedComponentId(typeBuilder, method, lazyInitializerField);
			}
			else
			{
				ImplementCallMethodOnImplementation(typeBuilder, method, lazyInitializerField);
			}
		}

		private static void ImplementConstructor(TypeBuilder typeBuilder, System.Type parentType, FieldInfo lazyInitializerField, FieldInfo proxyInfoField)
		{
			var constructor = typeBuilder.DefineConstructor(constructorAttributes, CallingConventions.Standard, new[] {LazyInitializerType, typeof(NHibernateProxyFactoryInfo)});

			var baseConstructor = parentType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, System.Type.EmptyTypes, null);

			// if there is no default constructor, or the default constructor is private/internal, call System.Object constructor
			// this works, but the generated assembly will fail PeVerify (cannot use in medium trust for example)
			if (baseConstructor == null || baseConstructor.IsPrivate || baseConstructor.IsAssembly)
				baseConstructor = ObjectConstructor;

			var IL = constructor.GetILGenerator();

			constructor.SetImplementationFlags(MethodImplAttributes.IL | MethodImplAttributes.Managed);

			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Call, baseConstructor);

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

			constructor.SetImplementationFlags(MethodImplAttributes.IL | MethodImplAttributes.Managed);

			var baseConstructor = parentType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, System.Type.EmptyTypes, null);
			// if there is no default constructor, or the default constructor is private/internal, call System.Object constructor
			// this works, but the generated assembly will fail PeVerify (cannot use in medium trust for example)
			if (baseConstructor == null || baseConstructor.IsPrivate || baseConstructor.IsAssembly)
				baseConstructor = ObjectConstructor;
			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Call, baseConstructor);

			//Everything is done in NHibernateProxyObjectReference, so just return data.
			IL.Emit(OpCodes.Ret);
		}

		private static void ImplementGetObjectData(TypeBuilder typeBuilder, FieldInfo proxyInfoField, FieldInfo lazyInitializerField)
		{
			const MethodAttributes attributes =
				MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual;
			
			var parameterTypes = new[] {typeof (SerializationInfo), typeof (StreamingContext)};

			var methodBuilder = typeBuilder.DefineMethod("GetObjectData", attributes, typeof (void), parameterTypes);
			methodBuilder.SetImplementationFlags(MethodImplAttributes.IL | MethodImplAttributes.Managed);
			methodBuilder.SetCustomAttribute(new CustomAttributeBuilder(SecurityCriticalAttributeConstructor, Array.Empty<object>()));

			var IL = methodBuilder.GetILGenerator();
			//LocalBuilder proxyBaseType = IL.DeclareLocal(typeof(Type));

			// info.SetType(typeof(NHibernateProxyObjectReference));
			IL.Emit(OpCodes.Ldarg_1);
			IL.Emit(OpCodes.Ldtoken, typeof (NHibernateProxyObjectReference));
			IL.Emit(OpCodes.Call, ReflectionCache.TypeMethods.GetTypeFromHandle);
			IL.Emit(OpCodes.Callvirt, SerializationInfoSetTypeMethod);

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
			IL.Emit(OpCodes.Callvirt, SerializableGetObjectDataMethod);

			IL.Emit(OpCodes.Ret);

			typeBuilder.DefineMethodOverride(methodBuilder, SerializableGetObjectDataMethod);
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

		private static void ImplementGetIdentifier(TypeBuilder typeBuilder, MethodInfo method, FieldInfo lazyInitializerField)
		{
			/*
			get 
			{
				if (this.__lazyInitializer == null)
					return base.get_<Identifier>();
				return (<ReturnType>)this.__lazyInitializer.Identifier;
			}
			 */
			var methodOverride = DefaultyProxyMethodBuilder.GenerateMethodSignature(method.Name, method, typeBuilder);

			var IL = methodOverride.GetILGenerator();

			EmitCallBaseIfLazyInitializerIsNull(IL, method, lazyInitializerField);

			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldfld, lazyInitializerField);
			IL.Emit(OpCodes.Callvirt, LazyInitializerIdentifierProperty.GetMethod);
			IL.Emit(OpCodes.Unbox_Any, method.ReturnType);
			IL.Emit(OpCodes.Ret);

			typeBuilder.DefineMethodOverride(methodOverride, method);
		}

		private static void ImplementSetIdentifier(TypeBuilder typeBuilder, MethodInfo method, FieldInfo lazyInitializerField)
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
			var methodOverride = DefaultyProxyMethodBuilder.GenerateMethodSignature(method.Name, method, typeBuilder);
			var IL = methodOverride.GetILGenerator();

			EmitCallBaseIfLazyInitializerIsNull(IL, method, lazyInitializerField);

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

		private static void ImplementCallMethodOnEmbeddedComponentId(TypeBuilder typeBuilder, MethodInfo method, FieldInfo lazyInitializerField)
		{
			/*
			 	if (this.__lazyInitializer == null)
					return base.<Method>(args..);
				this.__lazyInitializer.Identifier.<Method>(args..);
			*/
			var methodOverride = DefaultyProxyMethodBuilder.GenerateMethodSignature(method.Name, method, typeBuilder);

			var IL = methodOverride.GetILGenerator();

			EmitCallBaseIfLazyInitializerIsNull(IL, method, lazyInitializerField);

			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldfld, lazyInitializerField);
			IL.Emit(OpCodes.Callvirt, LazyInitializerIdentifierProperty.GetMethod);
			IL.Emit(OpCodes.Unbox_Any, method.DeclaringType);
			EmitCallMethod(IL, OpCodes.Callvirt, method);
			IL.Emit(OpCodes.Ret);

			typeBuilder.DefineMethodOverride(methodOverride, method);
		}

		private static void ImplementCallMethodOnImplementation(TypeBuilder typeBuilder, MethodInfo method, FieldInfo lazyInitializerField)
		{
			/*
				if (this.__lazyInitializer == null)
					return base.<Method>(args..);
				return this.__lazyInitializer.GetImplementation().<Method>(args..) 
			 */
			var methodOverride = DefaultyProxyMethodBuilder.GenerateMethodSignature(method.Name, method, typeBuilder);

			var IL = methodOverride.GetILGenerator();

			EmitCallBaseIfLazyInitializerIsNull(IL, method, lazyInitializerField);

			EmitCallImplementation(IL, method, lazyInitializerField);
			IL.Emit(OpCodes.Ret);

			typeBuilder.DefineMethodOverride(methodOverride, method);
		}

		private static void EmitCallBaseIfLazyInitializerIsNull(ILGenerator IL, MethodInfo method, FieldInfo lazyInitializerField)
		{
			//if (this.__lazyInitializer == null)
			//	return base.<Method>(args..)

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
