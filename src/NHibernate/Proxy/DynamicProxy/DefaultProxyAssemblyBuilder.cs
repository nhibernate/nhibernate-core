using System;
using System.Reflection;
using System.Reflection.Emit;

namespace NHibernate.Proxy.DynamicProxy
{
	public class DefaultProxyAssemblyBuilder : IProxyAssemblyBuilder
	{
		private AssemblyBuilder assemblyBuilder;

		public AssemblyBuilder DefineDynamicAssembly(AppDomain appDomain, AssemblyName name)
		{
#if DEBUG
			AssemblyBuilderAccess access = AssemblyBuilderAccess.RunAndSave;
#else
			AssemblyBuilderAccess access = AssemblyBuilderAccess.Run;
#endif
			assemblyBuilder = appDomain.DefineDynamicAssembly(name, access);
			return assemblyBuilder;
		}

		public ModuleBuilder DefineDynamicModule(string moduleName)
		{
#if DEBUG
			ModuleBuilder moduleBuilder =
				assemblyBuilder.DefineDynamicModule(moduleName, string.Format("{0}.mod", moduleName), true);
#else
			ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(moduleName);
#endif
			return moduleBuilder;
		}

		public void Save()
		{
#if DEBUG_PROXY_OUTPUT
			assemblyBuilder.Save("generatedAssembly.dll");
#endif
		}
	}
}
