using System;
using System.Reflection;
using System.Reflection.Emit;

namespace NHibernate.Proxy.DynamicProxy
{
	public class DefaultProxyAssemblyBuilder : IProxyAssemblyBuilder
	{
		public AssemblyBuilder DefineDynamicAssembly(AppDomain appDomain, AssemblyName name)
		{
#if DEBUG
			AssemblyBuilderAccess access = AssemblyBuilderAccess.RunAndSave;
#else
			AssemblyBuilderAccess access = AssemblyBuilderAccess.Run;
#endif
			return appDomain.DefineDynamicAssembly(name, access);
		}

		public ModuleBuilder DefineDynamicModule(AssemblyBuilder assemblyBuilder, string moduleName)
		{
#if DEBUG
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
