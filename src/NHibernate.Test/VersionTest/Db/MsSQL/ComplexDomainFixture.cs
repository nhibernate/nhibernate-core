using System.Collections;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.VersionTest.Db.MsSQL
{
	[TestFixture]
	public class ComplexDomainFixture : TestCase
	{
		protected override IList Mappings
		{
			get { return new[] { "VersionTest.Db.MsSQL.ComplexVersioned.hbm.xml" }; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is MsSql2000Dialect;
		}
		
		[Test]
		public void NH1685()
		{
			using (ISession session = OpenSession())
			{
				var bar = new Bar {AField = 24};

				var foo = new Foo {AField = 42};
				foo.AddBar(bar);

				session.Save(foo);
				session.Flush();
				session.Evict(bar);
				session.Evict(foo);

				var retrievedBar = session.Get<Bar>(bar.Id);

				// At this point the assumption is that bar and retrievedBar should have 
				// identical values, but represent two different POCOs. The asserts below 
				// are intended to verify this. Currently this test fails on the comparison 
				// of the SQL Server timestamp (i.e. binary(8)) fields because 
				// NHibernate does not retrieve the new timestamp after the last update. 

				Assert.AreNotSame(bar, retrievedBar);
				Assert.AreEqual(bar.Id, retrievedBar.Id);
				Assert.AreEqual(bar.AField, retrievedBar.AField);
				Assert.AreEqual(bar.Foo.Id, retrievedBar.Foo.Id);
				Assert.IsTrue(BinaryTimestamp.Equals(bar.Timestamp, retrievedBar.Timestamp), "Timestamps are different!");
			}

			using (ISession session = OpenSession())
			{
				session.BeginTransaction();
				session.Delete("from Bar");
				session.Transaction.Commit();
			}
		}
	}
}