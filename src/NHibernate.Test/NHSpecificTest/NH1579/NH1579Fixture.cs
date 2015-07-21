using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1579
{
	[TestFixture]
	public class NH1579Fixture : BugTestCase
	{
		[Test]
		public void Test()
		{
			Cart cart = new Cart("Fred");
			Apple apple = new Apple(cart);
			Orange orange = new Orange(cart);
			cart.Apples.Add(apple);
			cart.Oranges.Add(orange);

			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					session.Save(cart);
					tx.Commit();
				}
			}

			using (ISession session = OpenSession())
			{
				IQuery query = session.CreateQuery("FROM Fruit f WHERE f.Container.id = :containerID");
				query.SetGuid("containerID", cart.ID);
				IList<Fruit> fruit = query.List<Fruit>();
				Assert.AreEqual(2, fruit.Count);
			}

			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					session.Delete("FROM Entity");
					tx.Commit();
				}
			}
		}
	}
}
