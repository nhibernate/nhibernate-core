using System;
using System.Linq;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.IdTest
{
	[TestFixture]
	public class IdentityGeneratorNoPoolingFixture : IdFixtureBase
	{
		protected override string TypeName
		{
			get { return "Identity"; }
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect.SupportsIdentityColumns && dialect.SupportsPoolingParameter;
		}

		protected override void Configure(Configuration configuration)
		{
			var connectionString = configuration.GetProperty(Cfg.Environment.ConnectionString) +
				";Pooling=false";
			configuration.SetProperty(Cfg.Environment.ConnectionString, connectionString);
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.CreateQuery("delete System.Object").ExecuteUpdate();
				t.Commit();
			}
		}

		[Test(Description = "NH-3600")]
		public void NonTransactedInsert()
		{
			using (var s = OpenSession())
			{
				Assert.DoesNotThrow(() => s.Save(new IdentityClass()));
				// No need to flush with identity generator.
				Assert.AreEqual(1, s.Query<IdentityClass>().Count());
			}
		}

		[Test]
		public void TransactedInsert()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				Assert.DoesNotThrow(() => s.Save(new IdentityClass()));
				// No need to flush with identity generator.
				Assert.AreEqual(1, s.Query<IdentityClass>().Count());
				t.Commit();
			}
		}
	}
}