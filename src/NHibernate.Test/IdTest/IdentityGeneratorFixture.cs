using System;
using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.IdTest
{
	[TestFixture]
	public class IdentityGeneratorFixture : IdFixtureBase
	{
		protected override string TypeName
		{
			get { return "Identity"; }
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect.SupportsIdentityColumns;
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

		[Test(Description = "NH-926")]
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