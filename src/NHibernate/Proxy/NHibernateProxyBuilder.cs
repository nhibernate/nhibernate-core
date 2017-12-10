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
	class NHibernateProxyBuilder
	{
		const MethodAttributes InterceptorMethodsAttributes = MethodAttributes.Private | MethodAttributes.Final | MethodAttributes.HideBySig |
		                                                      MethodAttributes.SpecialName | MethodAttributes.NewSlot | MethodAttributes.Virtual;

		const MethodAttributes constructorAttributes = MethodAttributes.Public |
		                                               MethodAttributes.HideBySig | MethodAttributes.SpecialName |
		                                               MethodAttributes.RTSpecialName;

		static readonly System.Type NHibernateProxyType = typeof(INHibernateProxy);
		static readonly PropertyInfo NHibernateProxyTypeLazyInitializerProperty = NHibernateProxyType.GetProperty(nameof(INHibernateProxy.HibernateLazyInitializer));
		static readonly System.Type LazyInitializerType = typeof(ILazyInitializer);
		static readonly PropertyInfo LazyInitializerIdentifierProperty = LazyInitializerType.GetProperty(nameof(ILazyInitializer.Identifier));
		static readonly MethodInfo LazyInitializerInitializeMethod = LazyInitializerType.GetMethod(nameof(ILazyInitializer.Initialize));
		static readonly MethodInfo LazyInitializerGetImplementationMethod = LazyInitializerType.GetMethod(nameof(ILazyInitializer.GetImplementation), System.Type.EmptyTypes);
		static readonly IProxyAssemblyBuilder ProxyAssemblyBuilder = new DefaultProxyAssemblyBuilder();

		static readonly ConstructorInfo SecurityCriticalAttributeConstructor = typeof(SecurityCriticalAttribute).GetConstructor(System.Type.EmptyTypes);
		static readonly MethodInfo SerializableGetObjectDataMethod = typeof(ISerializable).GetMethod(nameof(ISerializable.GetObjectData));

		readonly MethodInfo _getIdentifierMethod;
		readonly MethodInfo _setIdentifierMethod;
		readonly IAbstractComponentType _componentIdType;
		readonly bool _overridesEquals;
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
			var customAttributeBuilder = new CustomAttributeBuilder(serializableConstructor, new object[0]);
			typeBuilder.SetCustomAttribute(customAttributeBuilder);

			ImplementDeserializationConstructor(typeBuilder);
			ImplementGetObjectData(typeBuilder, proxyInfoField, lazyInitializerField);

			var proxyType = typeBuilder.CreateTypeInfo();

			ProxyAssemblyBuilder.Save(assemblyBuilder);

			return proxyType;
		}

		void CreateProxiedMethod(TypeBuilder typeBuilder, MethodInfo method, FieldInfo lazyInitializerField)
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
		
		static void ImplementConstructor(TypeBuilder typeBuilder, System.Type parentType, FieldInfo lazyInitializerField, FieldInfo proxyInfoField)
		{
			var constructor = typeBuilder.DefineConstructor(constructorAttributes, CallingConventions.Standard, new[] {LazyInitializerType, typeof(NHibernateProxyFactoryInfo)});

			var baseConstructor = parentType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, System.Type.EmptyTypes, null);

			// if there is no default constructor, or the default constructor is private/internal, call System.Object constructor
			// this works, but the generated assembly will fail PeVerify (cannot use in medium trust for example)
			if (baseConstructor == null || baseConstructor.IsPrivate || baseConstructor.IsAssembly)
				baseConstructor = ProxyFactory.defaultBaseConstructor;

			var IL = constructor.GetILGenerator();

			constructor.SetImplementationFlags(MethodImplAttributes.IL | MethodImplAttributes.Managed);

			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Call, baseConstructor);

			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldarg_1);
			IL.Emit(OpCodes.Stfld, lazyInitializerField);

			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldarg_2);
			IL.Emit(OpCodes.Stfld, proxyInfoField);

			IL.Emit(OpCodes.Ret);
		}

		static void ImplementDeserializationConstructor(TypeBuilder typeBuilder)
		{
			var parameterTypes = new[] {typeof (SerializationInfo), typeof (StreamingContext)};
			var constructor = typeBuilder.DefineConstructor(constructorAttributes, CallingConventions.Standard, parameterTypes);
			constructor.SetImplementationFlags(MethodImplAttributes.IL | MethodImplAttributes.Managed);

			var IL = constructor.GetILGenerator();
			//Everything is done in NHibernateProxyObjectReference, so just return data.
			IL.Emit(OpCodes.Ret);
		}

		static void ImplementGetObjectData(TypeBuilder typeBuilder, FieldInfo proxyInfoField, FieldInfo lazyInitializerField)
		{
			const MethodAttributes attributes = MethodAttributes.Public | MethodAttributes.HideBySig |
			                                    MethodAttributes.Virtual;
			var parameterTypes = new[] {typeof (SerializationInfo), typeof (StreamingContext)};

			var methodBuilder = typeBuilder.DefineMethod("GetObjectData", attributes, typeof (void), parameterTypes);
			methodBuilder.SetCustomAttribute(new CustomAttributeBuilder(SecurityCriticalAttributeConstructor, Array.Empty<object>()));

			var IL = methodBuilder.GetILGenerator();
			//LocalBuilder proxyBaseType = IL.DeclareLocal(typeof(Type));

			// info.SetType(typeof(NHibernateProxyObjectReference));
			IL.Emit(OpCodes.Ldarg_1);
			IL.Emit(OpCodes.Ldtoken, typeof (NHibernateProxyObjectReference));
			IL.Emit(OpCodes.Call, ReflectionCache.TypeMethods.GetTypeFromHandle);
			IL.Emit(OpCodes.Callvirt, ProxyFactory.setType);

			//this.__proxyInfo
			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldfld, proxyInfoField);

			//this.LazyInitializer.Identifier
			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldfld, lazyInitializerField);
			IL.Emit(OpCodes.Callvirt, LazyInitializerIdentifierProperty.GetMethod);

			var constructor = typeof(NHibernateProxyObjectReference).GetConstructor(
				new[]
				{
					typeof(NHibernateProxyFactoryInfo),
					typeof(object),
				});
			IL.Emit(OpCodes.Newobj, constructor);

			IL.Emit(OpCodes.Ldarg_1);
			IL.Emit(OpCodes.Ldarg_2);

			IL.Emit(OpCodes.Callvirt, SerializableGetObjectDataMethod);

			IL.Emit(OpCodes.Ret);

			typeBuilder.DefineMethodOverride(methodBuilder, SerializableGetObjectDataMethod);
		}

		static void ImplementGetLazyInitializer(TypeBuilder typeBuilder, MethodInfo method, FieldInfo lazyInitializerField)
		{
			// Implement the getter
			var getMethod = typeBuilder.DefineMethod($"{NHibernateProxyType.FullName}.get_{nameof(INHibernateProxy.HibernateLazyInitializer)}", InterceptorMethodsAttributes, CallingConventions.HasThis, LazyInitializerType, System.Type.EmptyTypes);
			getMethod.SetImplementationFlags(MethodImplAttributes.Managed | MethodImplAttributes.IL);

			var IL = getMethod.GetILGenerator();

			// This is equivalent to:
			// get { return __lazyInitializer; }
			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldfld, lazyInitializerField);
			IL.Emit(OpCodes.Ret);

			typeBuilder.DefineMethodOverride(getMethod, method);
		}

		static void ImplementGetIdentifier(TypeBuilder typeBuilder, MethodInfo method, FieldInfo lazyInitializerField)
		{
			// get => return (ReturnType)(this.__lazyInitializer.Identifier;
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

		static void ImplementSetIdentifier(TypeBuilder typeBuilder, MethodInfo method, FieldInfo lazyInitializerField)
		{
			/*
			 set 
			 {
				(this.__lazyInitializer.Initialize();
				(this.__lazyInitializer.Identifier = value;
				(this.__lazyInitializer.GetImplementation().<Identifier> = value;
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

		static void ImplementCallMethodOnEmbeddedComponentId(TypeBuilder typeBuilder, MethodInfo method, FieldInfo lazyInitializerField)
		{
			// (this.__lazyInitializer.Identifier.<Method>(args..);
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

		static void ImplementCallMethodOnImplementation(TypeBuilder typeBuilder, MethodInfo method, FieldInfo lazyInitializerField)
		{
			/*
				return (this.__lazyInitializer.GetImplementation().<Method>(args..);
			*/
			var methodOverride = DefaultyProxyMethodBuilder.GenerateMethodSignature(method.Name, method, typeBuilder);

			var IL = methodOverride.GetILGenerator();

			EmitCallBaseIfLazyInitializerIsNull(IL, method, lazyInitializerField);

			EmitCallImplementation(IL, method, lazyInitializerField);
			IL.Emit(OpCodes.Ret);

			typeBuilder.DefineMethodOverride(methodOverride, method);
		}

		static void EmitCallBaseIfLazyInitializerIsNull(ILGenerator IL, MethodInfo method, FieldInfo lazyInitializerField)
		{
			//if ((this.__lazyInitializer == null)
			//	return base.<Method> (args..)

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

		static void EmitCallMethod(ILGenerator IL, OpCode opCode, MethodInfo method)
		{
			for (var i = 0; i < method.GetParameters().Length; i++)
				IL.Emit(OpCodes.Ldarg_S, (sbyte) (1 + i));
			IL.Emit(opCode, method);
		}

		static void EmitCallImplementation(ILGenerator IL, MethodInfo method, FieldInfo lazyInitializerField)
		{
			//(this.__lazyInitializer.GetImplementation().<Method>(args..);
			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldfld, lazyInitializerField);
			IL.Emit(OpCodes.Callvirt, LazyInitializerGetImplementationMethod);
			IL.Emit(OpCodes.Unbox_Any, method.DeclaringType);
			EmitCallMethod(IL, OpCodes.Callvirt, method);
		}
	}
}
