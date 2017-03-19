using System;
using System.Reflection;
using System.Reflection.Emit;

namespace NHibernate.Proxy.DynamicProxy
{
	public interface IProxyAssemblyBuilder
	{
		AssemblyBuilder DefineDynamicAssembly(AssemblyName name);
		ModuleBuilder DefineDynamicModule(AssemblyBuilder assemblyBuilder, string moduleName);
		void Save(AssemblyBuilder assemblyBuilder);
	}
}
