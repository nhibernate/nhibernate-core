#region Credits

// This work is based on LinFu.DynamicProxy framework (c) Philip Laureano who has donated it to NHibernate project.
// The license is the same of NHibernate license (LGPL Version 2.1, February 1999).
// The source was then modified to be the default DynamicProxy of NHibernate project.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using NHibernate.Linq;
using NHibernate.Util;

namespace NHibernate.Proxy.DynamicProxy
{
	public sealed class ProxyFactory
	{
		private static readonly ConstructorInfo defaultBaseConstructor = typeof(object).GetConstructor(new System.Type[0]);

		private static readonly MethodInfo getValue = ReflectHelper.GetMethod<SerializationInfo>(
			si => si.GetValue(null, null));
		private static readonly MethodInfo setType = ReflectHelper.GetMethod<SerializationInfo>(
			si => si.SetType(null));
		private static readonly MethodInfo addValue = ReflectHelper.GetMethod<SerializationInfo>(
			si => si.AddValue(null, null));

		public ProxyFactory()
			: this(new DefaultyProxyMethodBuilder()) {}

		public ProxyFactory(IProxyAssemblyBuilder proxyAssemblyBuilder)
			: this(new DefaultyProxyMethodBuilder(), proxyAssemblyBuilder) {}

		public ProxyFactory(IProxyMethodBuilder proxyMethodBuilder)
			: this(new DefaultyProxyMethodBuilder(), new DefaultProxyAssemblyBuilder()) {}

		public ProxyFactory(IProxyMethodBuilder proxyMethodBuilder, IProxyAssemblyBuilder proxyAssemblyBuilder)
		{
			if (proxyMethodBuilder == null)
			{
				throw new ArgumentNullException("proxyMethodBuilder");
			}
			ProxyMethodBuilder = proxyMethodBuilder;
			ProxyAssemblyBuilder = proxyAssemblyBuilder;
			Cache = new ProxyCache();
		}

		public IProxyCache Cache { get; private set; }

		public IProxyMethodBuilder ProxyMethodBuilder { get; private set; }

		public IProxyAssemblyBuilder ProxyAssemblyBuilder { get; private set; }

		public object CreateProxy(System.Type instanceType, IInterceptor interceptor, params System.Type[] baseInterfaces)
		{
			System.Type proxyType = CreateProxyType(instanceType, baseInterfaces);
			object result = Activator.CreateInstance(proxyType);
			var proxy = (IProxy) result;
			proxy.Interceptor = interceptor;

			return result;
		}

		public System.Type CreateProxyType(System.Type baseType, params System.Type[] interfaces)
		{
			System.Type[] baseInterfaces = ReferenceEquals(null, interfaces) ? new System.Type[0] : interfaces.Where(t => t != null).ToArray();
			
			TypeInfo proxyTypeInfo;

			// Reuse the previous results, if possible, Fast path without locking.
			if (Cache.TryGetProxyType(baseType, baseInterfaces, out proxyTypeInfo))
				return proxyTypeInfo;

			lock (Cache)
			{
				// Recheck in case we got interrupted.
				if (!Cache.TryGetProxyType(baseType, baseInterfaces, out proxyTypeInfo))
				{
					proxyTypeInfo = CreateUncachedProxyType(baseType, baseInterfaces);

					// Cache the proxy type
					if (proxyTypeInfo != null && Cache != null)
						Cache.StoreProxyType(proxyTypeInfo, baseType, baseInterfaces);
				}

				return proxyTypeInfo;
			}
		}

		private TypeInfo CreateUncachedProxyType(System.Type baseType, System.Type[] baseInterfaces)
		{
			AppDomain currentDomain = AppDomain.CurrentDomain;
			string typeName = string.Format("{0}Proxy", baseType.Name);
			string assemblyName = string.Format("{0}Assembly", typeName);
			string moduleName = string.Format("{0}Module", typeName);

			var name = new AssemblyName(assemblyName);
			AssemblyBuilder assemblyBuilder = ProxyAssemblyBuilder.DefineDynamicAssembly(currentDomain, name);
			ModuleBuilder moduleBuilder = ProxyAssemblyBuilder.DefineDynamicModule(assemblyBuilder, moduleName);

			TypeAttributes typeAttributes = TypeAttributes.AutoClass | TypeAttributes.Class |
											TypeAttributes.Public | TypeAttributes.BeforeFieldInit;

			var interfaces = new HashSet<System.Type>();
			interfaces.Merge(baseInterfaces);

			// Use the proxy dummy as the base type 
			// since we're not inheriting from any class type
			System.Type parentType = baseType;
			if (baseType.IsInterface)
			{
				parentType = typeof (ProxyDummy);
				interfaces.Add(baseType);
			}

			// Add any inherited interfaces
			System.Type[] computedInterfaces = interfaces.ToArray();
			foreach (System.Type interfaceType in computedInterfaces)
			{
				interfaces.Merge(GetInterfaces(interfaceType));
			}

			// Add the ISerializable interface so that it can be implemented
			interfaces.Add(typeof (ISerializable));

			TypeBuilder typeBuilder = moduleBuilder.DefineType(typeName, typeAttributes, parentType, interfaces.ToArray());

			ConstructorBuilder defaultConstructor = DefineConstructor(typeBuilder, parentType);

			// Implement IProxy
			var implementor = new ProxyImplementor();
			implementor.ImplementProxy(typeBuilder);

			FieldInfo interceptorField = implementor.InterceptorField;
			
			// Provide a custom implementation of ISerializable
			// instead of redirecting it back to the interceptor
			foreach (MethodInfo method in GetProxiableMethods(baseType, interfaces).Where(method => method.DeclaringType != typeof(ISerializable)))
			{
				ProxyMethodBuilder.CreateProxiedMethod(interceptorField, method, typeBuilder);
			}

			// Make the proxy serializable
			AddSerializationSupport(baseType, baseInterfaces, typeBuilder, interceptorField, defaultConstructor);
			TypeInfo proxyType = typeBuilder.CreateTypeInfo();

			ProxyAssemblyBuilder.Save(assemblyBuilder);
			return proxyType;
		}

		private IEnumerable<System.Type> GetInterfaces(System.Type currentType)
		{
			return GetAllInterfaces(currentType);
		}

		private IEnumerable<System.Type> GetAllInterfaces(System.Type currentType)
		{
			System.Type[] interfaces = currentType.GetInterfaces();

			foreach (System.Type current in interfaces)
			{
				yield return current;
				foreach (System.Type @interface in GetAllInterfaces(current))
				{
					yield return @interface;
				}
			}
		}

		private IEnumerable<MethodInfo> GetProxiableMethods(System.Type type, IEnumerable<System.Type> interfaces)
		{
			const BindingFlags candidateMethodsBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
			return 
				type.GetMethods(candidateMethodsBindingFlags)
					.Where(method=> method.IsProxiable())
					.Concat(interfaces.SelectMany(interfaceType => interfaceType.GetMethods())).Distinct();
		}

		private static ConstructorBuilder DefineConstructor(TypeBuilder typeBuilder, System.Type parentType)
		{
			const MethodAttributes constructorAttributes = MethodAttributes.Public |
														   MethodAttributes.HideBySig | MethodAttributes.SpecialName |
														   MethodAttributes.RTSpecialName;

			ConstructorBuilder constructor =
				typeBuilder.DefineConstructor(constructorAttributes, CallingConventions.Standard, new System.Type[0]);

			var baseConstructor = parentType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, new System.Type[0], null);

			// if there is no default constructor, or the default constructor is private/internal, call System.Object constructor
			// this works, but the generated assembly will fail PeVerify (cannot use in medium trust for example)
			if (baseConstructor == null || baseConstructor.IsPrivate || baseConstructor.IsAssembly)
				baseConstructor = defaultBaseConstructor;

			ILGenerator IL = constructor.GetILGenerator();

			constructor.SetImplementationFlags(MethodImplAttributes.IL | MethodImplAttributes.Managed);

			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Call, baseConstructor);
			IL.Emit(OpCodes.Ret);

			return constructor;
		}

		private static void ImplementGetObjectData(System.Type baseType, System.Type[] baseInterfaces, TypeBuilder typeBuilder, FieldInfo interceptorField)
		{
			const MethodAttributes attributes = MethodAttributes.Public | MethodAttributes.HideBySig |
												MethodAttributes.Virtual;
			var parameterTypes = new[] {typeof (SerializationInfo), typeof (StreamingContext)};

			MethodBuilder methodBuilder =
				typeBuilder.DefineMethod("GetObjectData", attributes, typeof (void), parameterTypes);

			ILGenerator IL = methodBuilder.GetILGenerator();
			//LocalBuilder proxyBaseType = IL.DeclareLocal(typeof(Type));

			// info.SetType(typeof(ProxyObjectReference));
			IL.Emit(OpCodes.Ldarg_1);
			IL.Emit(OpCodes.Ldtoken, typeof (ProxyObjectReference));
			IL.Emit(OpCodes.Call, ReflectionCache.TypeMethods.GetTypeFromHandle);
			IL.Emit(OpCodes.Callvirt, setType);

			// info.AddValue("__interceptor", __interceptor);
			IL.Emit(OpCodes.Ldarg_1);
			IL.Emit(OpCodes.Ldstr, "__interceptor");
			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldfld, interceptorField);
			IL.Emit(OpCodes.Callvirt, addValue);

			IL.Emit(OpCodes.Ldarg_1);
			IL.Emit(OpCodes.Ldstr, "__baseType");
			IL.Emit(OpCodes.Ldstr, baseType.AssemblyQualifiedName);
			IL.Emit(OpCodes.Callvirt, addValue);

			int baseInterfaceCount = baseInterfaces.Length;

			// Save the number of base interfaces
			IL.Emit(OpCodes.Ldarg_1);
			IL.Emit(OpCodes.Ldstr, "__baseInterfaceCount");
			IL.Emit(OpCodes.Ldc_I4, baseInterfaceCount);
			IL.Emit(OpCodes.Box, typeof (Int32));
			IL.Emit(OpCodes.Callvirt, addValue);

			int index = 0;
			foreach (System.Type baseInterface in baseInterfaces)
			{
				IL.Emit(OpCodes.Ldarg_1);
				IL.Emit(OpCodes.Ldstr, string.Format("__baseInterface{0}", index++));
				IL.Emit(OpCodes.Ldstr, baseInterface.AssemblyQualifiedName);
				IL.Emit(OpCodes.Callvirt, addValue);
			}

			IL.Emit(OpCodes.Ret);
		}

		private static void DefineSerializationConstructor(TypeBuilder typeBuilder, FieldInfo interceptorField, ConstructorBuilder defaultConstructor)
		{
			const MethodAttributes constructorAttributes = MethodAttributes.Public |
														   MethodAttributes.HideBySig | MethodAttributes.SpecialName |
														   MethodAttributes.RTSpecialName;

			var parameterTypes = new[] {typeof (SerializationInfo), typeof (StreamingContext)};
			ConstructorBuilder constructor = typeBuilder.DefineConstructor(constructorAttributes,
																		   CallingConventions.Standard, parameterTypes);

			ILGenerator IL = constructor.GetILGenerator();

			LocalBuilder interceptorType = IL.DeclareLocal(typeof(System.Type));
			//LocalBuilder interceptor = IL.DeclareLocal(typeof(IInterceptor));

			constructor.SetImplementationFlags(MethodImplAttributes.IL | MethodImplAttributes.Managed);


			IL.Emit(OpCodes.Ldtoken, typeof (IInterceptor));
			IL.Emit(OpCodes.Call, ReflectionCache.TypeMethods.GetTypeFromHandle);
			IL.Emit(OpCodes.Stloc, interceptorType);

			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Call, defaultConstructor);

			// __interceptor = (IInterceptor)info.GetValue("__interceptor", typeof(IInterceptor));
			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Ldarg_1);
			IL.Emit(OpCodes.Ldstr, "__interceptor");
			IL.Emit(OpCodes.Ldloc, interceptorType);
			IL.Emit(OpCodes.Callvirt, getValue);
			IL.Emit(OpCodes.Castclass, typeof (IInterceptor));
			IL.Emit(OpCodes.Stfld, interceptorField);

			IL.Emit(OpCodes.Ret);
		}

		private static void AddSerializationSupport(System.Type baseType, System.Type[] baseInterfaces, TypeBuilder typeBuilder, FieldInfo interceptorField, ConstructorBuilder defaultConstructor)
		{
			ConstructorInfo serializableConstructor = typeof(SerializableAttribute).GetConstructor(new System.Type[0]);
			var customAttributeBuilder = new CustomAttributeBuilder(serializableConstructor, new object[0]);
			typeBuilder.SetCustomAttribute(customAttributeBuilder);

			DefineSerializationConstructor(typeBuilder, interceptorField, defaultConstructor);
			ImplementGetObjectData(baseType, baseInterfaces, typeBuilder, interceptorField);
		}
	}
}