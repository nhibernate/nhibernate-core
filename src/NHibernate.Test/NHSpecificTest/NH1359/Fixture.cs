using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NHibernate.Criterion;
using NHibernate.Transform;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1359
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH1359"; }
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();
			using(ISession session = OpenSession())
			{
				using(ITransaction tx = session.BeginTransaction())
				{
					session.Delete("from Person");
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
					Pet p = new Pet("Fido", "Dog", 25, e1);
					Pet p2 = new Pet("Biff", "Dog", 10, e1);
					s.Save(p);
					s.Save(p2);
					tx.Commit();
				}
			}
		}

		[Test]
		public void CanSetSubQueryProjectionFromDetachedCriteriaWithCountProjection()
		{
			using(ISession s = OpenSession())
			{
				// This query doesn't make sense at all
				DetachedCriteria dc = DetachedCriteria.For<Person>()
					.SetProjection(Projections.Count("Id"));

				ICriteria c = s.CreateCriteria(typeof(Person))
					.SetProjection(Projections.SubQuery(dc))
					.Add(Expression.Eq("Name", "Fred"));

				IList list = c.List();
				Assert.AreEqual(2, list.Count);
				foreach(object item in list)
				{
					Assert.AreEqual(5, item);
				}
			}
		}

		public class HeaviestPet
		{
			public string Name;
			public double Weight;
		}

		[Test]
		public void CanSubqueryRelatedObjectsNotInMainQuery()
		{
			using(ISession s = OpenSession())
			{
				DetachedCriteria dc = DetachedCriteria.For<Person>().CreateCriteria("Pets", "pets")
					.SetProjection(Projections.Max("pets.Weight"));
				ICriteria c = s.CreateCriteria(typeof(Person))
					.SetProjection(Projections.ProjectionList()
									.Add(Projections.SubQuery(dc), "Weight")
									.Add(Projections.Property("Name"), "Name"))
					.Add(Restrictions.Eq("Name", "Joe"));
				c.SetResultTransformer(Transformers.AliasToBean(typeof(HeaviestPet)));

				IList<HeaviestPet> list = c.List<HeaviestPet>();
				Assert.AreEqual(1, list.Count);
				foreach(HeaviestPet pet in list)
				{
					Assert.AreEqual("Joe", pet.Name);
					Assert.AreEqual(25, pet.Weight);
				}
			}
		}

		[Test]
		public void CanGetSelectSubqueryWithSpecifiedParameter()
		{
			using (ISession s = OpenSession())
			{
				DetachedCriteria dc = DetachedCriteria.For<Person>().Add(Restrictions.Eq("Name", "Joe"))
					.SetProjection(Projections.Max("Name"));
				ICriteria c = s.CreateCriteria(typeof(Person))
					.SetProjection(Projections.ProjectionList()
									.Add(Projections.SubQuery(dc), "Name"))
									.Add(Restrictions.Eq("Name", "Joe"));
				c.SetResultTransformer(Transformers.AliasToBean(typeof(HeaviestPet)));
				IList<HeaviestPet> list = c.List<HeaviestPet>();
				Assert.AreEqual(1, list.Count);
				foreach(HeaviestPet pet in list)
				{
					Assert.AreEqual("Joe", pet.Name);
				}
			}
		}


		[Test]
		public void CanPageAndSortResultsWithParametersAndFilters()
		{
			using(ISession s = OpenSession())
			{
				s.EnableFilter("ExampleFilter").SetParameter("WeightVal", 100);
				DetachedCriteria dc = DetachedCriteria.For<Person>().CreateCriteria("Pets", "pets")
					.SetProjection(Projections.Max("pets.Weight"))
					.Add(Restrictions.Eq("pets.Weight", 10.0));
				ICriteria c = s.CreateCriteria(typeof(Person))
					.SetProjection(Projections.ProjectionList()
									.Add(Projections.SubQuery(dc), "Weight")
									.Add(Projections.Property("Name"), "Name"))
					.Add(Restrictions.Eq("Name", "Joe"));

				c.SetResultTransformer(Transformers.AliasToBean(typeof(HeaviestPet)));
				c.SetMaxResults(1);
				c.AddOrder(new Order("Id", true));
				IList<HeaviestPet> list = c.List<HeaviestPet>();
				Assert.AreEqual(1, list.Count);
				foreach(HeaviestPet pet in list)
				{
					Assert.AreEqual("Joe", pet.Name);
					Assert.AreEqual(10.0, pet.Weight);
				}
			}
		}

		[Test]
		public void CanPageAndSortWithMultipleColumnsOfSameName()
		{
			using(ISession s = OpenSession())
			{
				ICriteria c = s.CreateCriteria(typeof(Person),"root")
					.CreateCriteria("root.Pets","pets")
					.SetProjection(Projections.ProjectionList()
									.Add(Projections.Property("root.Id"), "Id")
									.Add(Projections.Property("root.Name"), "Name"))
					.Add(Restrictions.Eq("Name", "Fido"));
				c.AddOrder(new Order("Id", true));
				c.SetResultTransformer(Transformers.AliasToBean(typeof(Person)));
				c.SetMaxResults(1);
				IList<Person> list = c.List<Person>();
				Assert.AreEqual(1, list.Count);
			}
		}

		[Test]
		public void CanOrderByNamedSubquery()
		{
			using(ISession s = OpenSession())
			{
				DetachedCriteria dc = DetachedCriteria.For<Person>().Add(Restrictions.Eq("Name", "Joe"))
					.SetProjection(Projections.Max("Name"));
				ICriteria c = s.CreateCriteria(typeof(Person))
					.SetProjection(Projections.ProjectionList()
									.Add(Projections.SubQuery(dc), "NameSubquery"))
									.Add(Restrictions.Eq("Name", "Joe"));
				c.AddOrder(new Order("NameSubquery", true));
				c.SetMaxResults(1);
				IList list = c.List();
				Assert.AreEqual(1, list.Count);
			}
		}
	}
}
