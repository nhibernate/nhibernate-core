using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Bytecode;
using NUnit.Framework;


namespace NHibernate.Test.GhostProperty
{
	// Since 5.3
	[Obsolete]
	[TestFixture]
	public class GhostPropertyDynamicProxyFixture : GhostPropertyFixture
	{
		protected override void Configure(Cfg.Configuration configuration)
		{
			base.Configure(configuration);
			configuration.SetProperty(Cfg.Environment.ProxyFactoryFactoryClass, typeof(DefaultProxyFactoryFactory).FullName);
		}
	}
}
