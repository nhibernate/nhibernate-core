using System.Collections;
using System.Collections.Generic;
using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1291AnonExample
{
	[TestFixture]
	public class NH1291AnonExampleFixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH1291AnonExample"; }
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();
			using(ISession session = OpenSession())
			{
				using(ITransaction tx = session.BeginTransaction())
				{
					session.Delete("from Person");
					session.Delete("from Home");
					tx.Commit();
				}
			}
		}

		protected override void OnSetUp()
		{
			using(ISession s = OpenSession())
			{
				using(ITransaction tx = s.BeginTransaction())
				{
					Person e1 = new Person("Joe", 10, 9);
					Person e2 = new Person("Sally", 20, 8);
					Person e3 = new Person("Tim", 20, 7); //20
					Person e4 = new Person("Fred", 40, 40);
					Person e5 = new Person("Fred", 50, 50);
					s.Save(e1);
					s.Save(e2);
					s.Save(e3);
					s.Save(e4);
					s.Save(e5);
					tx.Commit();
				}
			}
		}

		protected class PersonIQAnon
		{
			private int _iq;
			public PersonIQAnon(int iq)
			{
				IQ = iq;
			}

			public int IQ
			{
				get { return _iq; }
				set { _iq = value; }
			}
		}



		[Test]
		public void CanCreateAnonExampleForInt()
		{
			using(ISession s = OpenSession())
			{
				using(ITransaction tx = s.BeginTransaction())
				{
					IList list = s.CreateCriteria(typeof(Person))
						.Add(Example.Create(new PersonIQAnon(40))).List();
					//c# 3.5: Example.Create( new { IQ = 40 } )
					Assert.AreEqual(1, list.Count);
					Assert.AreEqual("Fred", ((Person)list[0]).Name);
					tx.Commit();
				}
			}
		}

		protected class PersonNameAnon
		{
			private string name;

			public PersonNameAnon(string name)
			{
				Name = name;
			}

			virtual public string Name
			{
				get { return name; }
				set { name = value; }
			}
		}

		[Test]
		public void CanCreateAnonExampleForStringLikeCompare()
		{
			using(ISession s = OpenSession())
			{
				using(ITransaction tx = s.BeginTransaction())
				{
					IList list = s.CreateCriteria(typeof(Person))
						.Add(Example.Create(new PersonNameAnon("%all%")).EnableLike()).List();
					//c# 3.5: Example.Create( new { Name = "%all%" } )
					Assert.AreEqual(1, list.Count);
					Assert.AreEqual("Sally", ((Person)list[0]).Name);
					tx.Commit();
				}
			}
		}

		[Test]
		public void CanQueryUsingSavedRelations()
		{
			using(ISession s = OpenSession())
			{
				using(ITransaction tx = s.BeginTransaction())
				{
					IList<Person> people = s.CreateCriteria(typeof(Person)).List<Person>();
					Home h1 = new Home("Eugene", 97402);
					Home h2 = new Home("Klamath Falls", 97603);
					people[0].Home = h1;
					people[1].Home = h2;
					s.Save(h1);
					s.Save(h2);

					IList list = s.CreateCriteria(typeof(Person)).CreateCriteria("Home")
						.Add(Example.Create(h1)).List();

					Assert.AreEqual(1, list.Count);
					Assert.AreEqual("Joe", ((Person)list[0]).Name);
					tx.Commit();
				}
			}
		}

		protected class HomeAnon
		{
			private int zip;

			public HomeAnon(int zip)
			{
				Zip = zip;
			}

			virtual public int Zip
			{
				get { return zip; }
				set { zip = value; }
			}
		}

		[Test]
		public void CanQueryUsingAnonRelations()
		{
			using(ISession s = OpenSession())
			{
				using(ITransaction tx = s.BeginTransaction())
				{
					IList<Person> people = s.CreateCriteria(typeof(Person)).List<Person>();
					Home h1 = new Home("Eugene", 97402);
					Home h2 = new Home("Klamath Falls", 97603);
					people[0].Home = h1;
					people[1].Home = h2;
					s.Save(h1);
					s.Save(h2);

					IList list = s.CreateCriteria(typeof(Person))
					    .CreateCriteria("Home").Add(Example.Create(new HomeAnon(97402))).List();
					//c# 3.5: Example.Create( new { Zip = 97402 } )

					Assert.AreEqual(1, list.Count);
					Assert.AreEqual("Joe", ((Person)list[0]).Name);
					tx.Commit();
				}
			}
		}
	}
}