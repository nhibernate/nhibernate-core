using System;
using System.Collections.Generic;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1028
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
            get { return "NH1028"; }
		}

		[Test]
		public void CanLoadCollectionUsingLeftOuterJoin()
		{
			String itemName = "a";
			String shipName = "blah";

			Item item = new Item();
			item.Name = itemName;

			Ship ship = new Ship();
			ship.Name = shipName;
			item.Ships.Add(ship);
			using (ISession s = OpenSession())
			{
				s.Save(ship);
				s.Save(item);
				s.Flush();
			}
			using (ISession s = OpenSession())
			{
				ICriteria criteria = s.CreateCriteria(typeof(Item));
				criteria.CreateCriteria("Ships", "s", JoinType.InnerJoin)
					 .Add(Expression.IsNotNull("s.Id"));
				criteria.CreateCriteria("Containers", "c", JoinType.LeftOuterJoin)
					.Add(Expression.IsNull("c.Id"));

				IList<Item> results = criteria.List<Item>();
				Assert.AreEqual (1, results.Count);

				Item loadedItem = results[0];
				Assert.AreEqual (itemName, loadedItem.Name);

				Assert.AreEqual (1, loadedItem.Ships.Count);
				foreach (Ship loadedShip in item.Ships) {
					Assert.AreEqual (shipName, loadedShip.Name);	
				}

				Assert.That(loadedItem.Containers, Is.Empty);
			}
			using (ISession s = OpenSession())
			{
				s.Delete(ship);
				s.Delete(item);
				s.Flush();
			}
		}
	}
}
