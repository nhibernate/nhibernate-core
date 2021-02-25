using System;
using System.Reflection;
using System.Reflection.Emit;

namespace NHibernate.Proxy.DynamicProxy
{
	// Since v5.2
	[Obsolete("DynamicProxy namespace has been obsoleted, use static proxies instead (see StaticProxyFactory)")]
	public interface IProxyAssemblyBuilder
	{
		AssemblyBuilder DefineDynamicAssembly(AppDomain appDomain, AssemblyName name);
		ModuleBuilder DefineDynamicModule(AssemblyBuilder assemblyBuilder, string moduleName);
		void Save(AssemblyBuilder assemblyBuilder);
	}
}
