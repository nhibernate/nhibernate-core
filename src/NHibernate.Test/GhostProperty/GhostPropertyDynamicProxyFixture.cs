using System;
using NHibernate.Bytecode;
using NUnit.Framework;

namespace NHibernate.Test.GhostProperty
{
	// Since 5.3
	[Obsolete]
	[TestFixture]
	public class GhostPropertyDynamicProxyFixture : GhostPropertyFixture
	{
		private string _originalProxyFactoryFactory;

		protected override void Configure(Cfg.Configuration configuration)
		{
			base.Configure(configuration);
			_originalProxyFactoryFactory = Cfg.Environment.BytecodeProvider.ProxyFactoryFactory.GetType().FullName;
			configuration.SetProperty(Cfg.Environment.ProxyFactoryFactoryClass, typeof(DefaultProxyFactoryFactory).FullName);
		}

		protected override void DropSchema()
		{
			base.DropSchema();
			// Reset IProxyFactoryFactory back to default
			var injectableProxyFactory = (IInjectableProxyFactoryFactory) Cfg.Environment.BytecodeProvider;
			injectableProxyFactory.SetProxyFactoryFactory(_originalProxyFactoryFactory);
		}
	}
}
