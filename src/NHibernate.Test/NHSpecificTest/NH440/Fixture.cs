using System;
using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH440
{
	/// <summary>
	///This is a test class for one_to_one_bug.Fruit and is intended
	///to contain all one_to_one_bug.Fruit Unit Tests
	///</summary>
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH440"; }
		}

		protected override IList Mappings
		{
			get { return new string[] {"NHSpecificTest.NH440.Fruit.hbm.xml", "NHSpecificTest.NH440.Apple.hbm.xml"}; }
		}


		protected override void OnSetUp()
		{
			base.OnSetUp();
			using (ISession session = OpenSession())
			using (ITransaction t = session.BeginTransaction())
			{
				// pump up the ids for one of the classes to avoid the tests passing coincidentally
				for (int i = 0; i < 10; i++)
				{
					session.Save(new Fruit());
				}

				session.Delete("from System.Object"); // clear everything from database
				t.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			using (ITransaction t = session.BeginTransaction())
			{
				session.Delete("from System.Object"); // clear everything from database
				t.Commit();
			}
			base.OnTearDown();
		}

		[Test]
		public void StoreAndLookup()
		{
			Apple apple = new Apple();
			Fruit fruit = new Fruit();

			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				session.Save(apple);
				session.Save(fruit);

				Assert.IsNotNull(session.Get(typeof(Apple), apple.Id));
				Assert.IsNotNull(session.Get(typeof(Fruit), fruit.Id));

				tx.Commit();
			}
		}

		[Test]
		public void StoreWithLinksAndLookup()
		{
			Apple apple = new Apple();
			Fruit fruit = new Fruit();

			apple.TheFruit = fruit;
			fruit.TheApple = apple;

			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				session.Save(apple);
				session.Save(fruit);

				tx.Commit();
			}

			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				Apple apple2 = (Apple) session.Get(typeof(Apple), apple.Id);
				Fruit fruit2 = (Fruit) session.Get(typeof(Fruit), fruit.Id);

				Assert.IsNotNull(apple2);
				Assert.IsNotNull(fruit2);

				Assert.AreSame(apple2, fruit2.TheApple);
				Assert.AreSame(fruit2, apple2.TheFruit);
				tx.Commit();
			}
		}

		[Test]
		public void StoreWithLinksAndLookupWithQueryFromFruit()
		{
			Apple apple = new Apple();
			Fruit fruit = new Fruit();

			apple.TheFruit = fruit;
			fruit.TheApple = apple;

			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				session.Save(apple);
				session.Save(fruit);
				tx.Commit();
			}

			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				Fruit fruit2 = (Fruit) session.Get(typeof(Fruit), fruit.Id);
				Assert.IsNotNull(fruit2);
				IList results = session
					.CreateQuery("from Apple a where a.TheFruit = ?")
					.SetParameter(0, fruit2)
					.List();

				Assert.AreEqual(1, results.Count);
				Apple apple2 = (Apple) results[0];
				Assert.IsNotNull(apple2);

				Assert.AreSame(apple2, fruit2.TheApple);
				Assert.AreSame(fruit2, apple2.TheFruit);
				tx.Commit();
			}
		}
	}
}