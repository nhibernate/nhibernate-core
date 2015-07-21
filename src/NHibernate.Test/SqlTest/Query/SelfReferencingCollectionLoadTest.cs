using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.SqlTest.Query
{
	[TestFixture]
	public class SelfReferencingCollectionLoadTest : TestCase
	{
		protected override IList Mappings
		{
			get { return new[] {"SqlTest.Query.Item.hbm.xml"}; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
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