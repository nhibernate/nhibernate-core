using System;
using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.SqlTest
{
	[TestFixture]
	public class SelfReferencingCollectionLoadTest : TestCase
	{
		protected override IList Mappings
		{
			get { return new string[] {"SqlTest.Item.hbm.xml"}; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		[Test]
		public void LoadCollection()
		{
			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				session.Save(new Item(1, 2));
				session.Save(new Item(2, 3));
				session.Save(new Item(3, 1));

				tx.Commit();
			}

			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				Item item1 = (Item) session.Get(typeof(Item), 1);
				Assert.AreEqual(2, item1.AlternativeItems.Count);

				session.Delete("from Item");

				tx.Commit();
			}
		}
	}
}