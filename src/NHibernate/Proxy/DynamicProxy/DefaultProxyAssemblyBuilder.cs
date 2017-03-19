using System;
using System.Reflection;
using System.Reflection.Emit;

namespace NHibernate.Proxy.DynamicProxy
{
	public class DefaultProxyAssemblyBuilder : IProxyAssemblyBuilder
	{
		public AssemblyBuilder DefineDynamicAssembly(AssemblyName name)
		{
#if FEATURE_APPDOMAIN || NET_4_0
#if DEBUG
			AssemblyBuilderAccess access = AssemblyBuilderAccess.RunAndSave;
#else
			AssemblyBuilderAccess access = AssemblyBuilderAccess.Run;
#endif
			AppDomain appDomain = AppDomain.CurrentDomain;
			return appDomain.DefineDynamicAssembly(name, access);
#else
			return AssemblyBuilder.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
#endif
		}

		public ModuleBuilder DefineDynamicModule(AssemblyBuilder assemblyBuilder, string moduleName)
		{
#if DEBUG && FEATURE_APPDOMAIN
			ModuleBuilder moduleBuilder =
				assemblyBuilder.DefineDynamicModule(moduleName, string.Format("{0}.mod", moduleName), true);
#else
			ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(moduleName);
#endif
			return moduleBuilder;
		}

		public void Save(AssemblyBuilder assemblyBuilder)
		{
#if DEBUG_PROXY_OUTPUT
			assemblyBuilder.Save("generatedAssembly.dll");
#endif
		}
	}
}
