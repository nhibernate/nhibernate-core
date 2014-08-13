using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.ConnectionTest
{
	[TestFixture]
	public class ResilientConnectionTestCase : ConnectionManagementTestCase
	{
		protected override void Configure(Configuration cfg)
		{
			base.Configure(cfg);
			cfg.SetProperty(NHibernate.Cfg.Environment.ConnectionProvider, typeof(ResilientDriverConnectionProvider).AssemblyQualifiedName);
		}

		[Test]
		public void TestResilientConnection()
		{
			using (var session = this.GetSessionUnderTest())
			{
				var sillies = session.Query<Silly>().ToList();

				Assert.IsInstanceOf<ResilientDriverConnectionProvider>(session.GetSessionImplementation().Factory.ConnectionProvider);
			}
		}

		protected override ISession GetSessionUnderTest()
		{
			return (this.OpenSession());
		}
	}
}
