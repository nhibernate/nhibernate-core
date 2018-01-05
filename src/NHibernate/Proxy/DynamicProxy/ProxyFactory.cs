#region Credits

// This work is based on LinFu.DynamicProxy framework (c) Philip Laureano who has donated it to NHibernate project.
// The license is the same of NHibernate license (LGPL Version 2.1, February 1999).
// The source was then modified to be the default DynamicProxy of NHibernate project.

#endregion

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using NHibernate.Util;

namespace NHibernate.Proxy.DynamicProxy
{
	public sealed class ProxyFactory
	{
		internal static readonly ConcurrentDictionary<ProxyCacheEntry, TypeInfo> _cache = new ConcurrentDictionary<ProxyCacheEntry, TypeInfo>();

		private static readonly ConstructorInfo defaultBaseConstructor = typeof(object).GetConstructor(System.Type.EmptyTypes);

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
			: this(proxyMethodBuilder, new DefaultProxyAssemblyBuilder()) {}

		public ProxyFactory(IProxyMethodBuilder proxyMethodBuilder, IProxyAssemblyBuilder proxyAssemblyBuilder)
		{
			ProxyMethodBuilder = proxyMethodBuilder ?? throw new ArgumentNullException(nameof(proxyMethodBuilder));
			ProxyAssemblyBuilder = proxyAssemblyBuilder;
		}

		//Since v5.1
		[Obsolete("This property is not used anymore and will be removed in a next major version")]
		public IProxyCache Cache { get; } = new ProxyCache();

		public IProxyMethodBuilder ProxyMethodBuilder { get; }

		public IProxyAssemblyBuilder ProxyAssemblyBuilder { get; }

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
			if (baseType == null) throw new ArgumentNullException(nameof(baseType));

			var typeFactory = _cache.GetOrAdd(
				new ProxyCacheEntry(baseType, interfaces),
				k => CreateUncachedProxyType(k.BaseType, k.Interfaces));

			return typeFactory;
		}

		private TypeInfo CreateUncachedProxyType(System.Type baseType, IReadOnlyCollection<System.Type> baseInterfaces)
		{
			AppDomain currentDomain = AppDomain.CurrentDomain;
			const InternLevel internLevel = InternLevel.ProxyType;
			string typeName = StringHelper.Intern($"{baseType.Name}Proxy", internLevel);
			string assemblyName = StringHelper.Intern($"{typeName}Assembly", internLevel);
			string moduleName = StringHelper.Intern($"{typeName}Module", internLevel);

			var name = new AssemblyName(assemblyName);
			AssemblyBuilder assemblyBuilder = ProxyAssemblyBuilder.DefineDynamicAssembly(currentDomain, name);
			ModuleBuilder moduleBuilder = ProxyAssemblyBuilder.DefineDynamicModule(assemblyBuilder, moduleName);

			TypeAttributes typeAttributes = TypeAttributes.AutoClass | TypeAttributes.Class |
			                                TypeAttributes.Public | TypeAttributes.BeforeFieldInit;

			var interfaces = new HashSet<System.Type>();
			interfaces.UnionWith(baseInterfaces);
			interfaces.UnionWith(baseInterfaces.SelectMany(i => i.GetInterfaces()));

			// Use the proxy dummy as the base type 
			// since we're not inheriting from any class type
			System.Type parentType = baseType;
			if (baseType.IsInterface)
			{
				parentType = typeof (ProxyDummy);
				interfaces.Add(baseType);
			}
			interfaces.UnionWith(baseType.GetInterfaces());

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

		internal static IEnumerable<MethodInfo> GetProxiableMethods(System.Type type, IEnumerable<System.Type> interfaces)
		{
			const BindingFlags candidateMethodsBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
			return
				type.GetMethods(candidateMethodsBindingFlags)
					.Where(method => method.IsProxiable())
					.Concat(interfaces.SelectMany(interfaceType => interfaceType.GetMethods()))
					.Distinct();
		}

		private static ConstructorBuilder DefineConstructor(TypeBuilder typeBuilder, System.Type parentType)
		{
			const MethodAttributes constructorAttributes = MethodAttributes.Public |
														   MethodAttributes.HideBySig | MethodAttributes.SpecialName |
														   MethodAttributes.RTSpecialName;

			ConstructorBuilder constructor =
				typeBuilder.DefineConstructor(constructorAttributes, CallingConventions.Standard, System.Type.EmptyTypes);

			var baseConstructor = parentType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, System.Type.EmptyTypes, null);

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

		private static void ImplementGetObjectData(System.Type baseType, IReadOnlyCollection<System.Type> baseInterfaces, TypeBuilder typeBuilder, FieldInfo interceptorField)
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

			int baseInterfaceCount = baseInterfaces.Count;

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

		private static void AddSerializationSupport(System.Type baseType, IReadOnlyCollection<System.Type> baseInterfaces, TypeBuilder typeBuilder, FieldInfo interceptorField, ConstructorBuilder defaultConstructor)
		{
			ConstructorInfo serializableConstructor = typeof(SerializableAttribute).GetConstructor(System.Type.EmptyTypes);
			var customAttributeBuilder = new CustomAttributeBuilder(serializableConstructor, Array.Empty<object>());
			typeBuilder.SetCustomAttribute(customAttributeBuilder);

			DefineSerializationConstructor(typeBuilder, interceptorField, defaultConstructor);
			ImplementGetObjectData(baseType, baseInterfaces, typeBuilder, interceptorField);
		}
	}
}
