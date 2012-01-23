using System;
using System.Reflection;
using System.Reflection.Emit;

namespace NHibernate.Proxy.DynamicProxy
{
	public interface IProxyAssemblyBuilder
	{
		AssemblyBuilder DefineDynamicAssembly(AppDomain appDomain, AssemblyName name);
		ModuleBuilder DefineDynamicModule(string moduleName);
		void Save();
	}
}
