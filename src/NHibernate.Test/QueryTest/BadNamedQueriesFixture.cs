using System;
using System.Collections;
using NHibernate.Cfg;
using NHibernate.Exceptions;
using NUnit.Framework;

namespace NHibernate.Test.QueryTest
{
	[TestFixture]
	public class BadNamedQueriesFixture : TestCase
	{
		protected override string[] Mappings => new[] { "QueryTest.BadNamedQueriesFixture.hbm.xml" };

		protected override string MappingsAssembly => "NHibernate.Test";

		protected override void Configure(Configuration configuration)
		{
			// Allow building of default session factory
			configuration.SetProperty(Cfg.Environment.QueryStartupChecking, "false");
		}

		protected override void OnSetUp()
		{
			cfg.SetProperty(Cfg.Environment.QueryStartupChecking, "true");
		}

		protected override void OnTearDown()
		{
			cfg.SetProperty(Cfg.Environment.QueryStartupChecking, "false");
		}

		[Test]
		public void StartupCheck()
		{
			Assert.That(
				BuildSessionFactory,
				Throws
					.InstanceOf<AggregateHibernateException>()
					.And.Property(nameof(AggregateHibernateException.InnerExceptions)).Count.EqualTo(2));
		}
	}
}
