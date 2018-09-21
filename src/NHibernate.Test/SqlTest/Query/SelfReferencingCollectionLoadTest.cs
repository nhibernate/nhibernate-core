using System.Collections;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.SqlTest.Query
{
	[TestFixture]
	public class SelfReferencingCollectionLoadTest : TestCase
	{
		protected override string[] Mappings
		{
			get { return new[] {"SqlTest.Query.Item.hbm.xml"}; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			// Hacky mapping causing the primary key to reference itself as a foreign key, which is not supported by
			// some databases. It fails when trying to insert data by considering the foreign key violated.
			return !(Dialect is MySQLDialect || Dialect is SapSQLAnywhere17Dialect);
		}

		[Test]
		public void LoadCollection()
		{
			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					session.Save(new Item(1, 2));
					session.Save(new Item(2, 3));
					session.Save(new Item(3, 1));

					tx.Commit();
				}
			}

			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					var item1 = (Item) session.Get(typeof (Item), 1);
					Assert.AreEqual(2, item1.AlternativeItems.Count);

					session.Delete("from Item");

					tx.Commit();
				}
			}
		}
	}
}
