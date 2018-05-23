using System;
using System.Collections;
using NHibernate.DomainModel;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NUnit.Framework;

namespace NHibernate.Test.Legacy
{
	[TestFixture]
	public class CriteriaTest : TestCase
	{
		protected override IList Mappings
		{
			get
			{
				return new string[]
					{
						"Simple.hbm.xml",
						"MasterDetail.hbm.xml"
					};
			}
		}

		[Test]
		public void SimpleSelectTest()
		{
			// create the objects to search on
			long simple1Key = 15;
			Simple simple1 = new Simple();
			simple1.Address = "Street 12";
			simple1.Date = RoundForDialect(DateTime.Now);
			simple1.Name = "For Criteria Test";
			simple1.Count = 16;

			long notSimple1Key = 17;
			Simple notSimple1 = new Simple();
			notSimple1.Address = "Street 123";
			notSimple1.Date = RoundForDialect(DateTime.Now);
			notSimple1.Name = "Don't be found";
			notSimple1.Count = 18;

			using (ISession s1 = OpenSession())
			using (ITransaction t1 = s1.BeginTransaction())
			{
				s1.Save(notSimple1, notSimple1Key);
				s1.Save(simple1, simple1Key);
				t1.Commit();
			}

			using (ISession s2 = OpenSession())
			using (ITransaction t2 = s2.BeginTransaction())
			{
				var results2 = s2.CreateCriteria<Simple>()
					.Add(Restrictions.Eq("Address", "Street 12"))
					.List<Simple>();

				Assert.That(results2.Count, Is.EqualTo(1), "Unexpected result count");

				var simple2 = results2[0];

				Assert.That(simple2, Is.Not.Null, "Unable to load object");
				Assert.That(simple2.Count, Is.EqualTo(simple1.Count), "Unexpected Count property value");
				Assert.That(simple2.Name, Is.EqualTo(simple1.Name), "Unexpected name");
				Assert.That(simple2.Address, Is.EqualTo(simple1.Address), "Unexpected address");
				Assert.That(simple2.Date, Is.EqualTo(simple1.Date), "Unexpected date");

				s2.Delete("from Simple");

				t2.Commit();
			}
		}

		[Test]
		public void SimpleDateCriteria()
		{
			Simple s1 = new Simple();
			s1.Address = "blah";
			s1.Count = 1;
			s1.Date = new DateTime(2004, 01, 01);

			Simple s2 = new Simple();
			s2.Address = "blah";
			s2.Count = 2;
			s2.Date = new DateTime(2006, 01, 01);

			using (ISession s = OpenSession())
			{
				s.Save(s1, 1L);
				s.Save(s2, 2L);
				s.Flush();
			}

			using (ISession s = OpenSession())
			{
				IList results = s.CreateCriteria(typeof(Simple))
					.Add(Expression.Gt("Date", new DateTime(2005, 01, 01)))
					.AddOrder(Order.Asc("Date"))
					.List();

				Assert.AreEqual(1, results.Count, "one gt from 2005");
				Simple simple = (Simple) results[0];
				Assert.IsTrue(simple.Date > new DateTime(2005, 01, 01), "should have returned dates after 2005");

				results = s.CreateCriteria(typeof(Simple))
					.Add(Expression.Lt("Date", new DateTime(2005, 01, 01)))
					.AddOrder(Order.Asc("Date"))
					.List();

				Assert.AreEqual(1, results.Count, "one lt than 2005");
				simple = (Simple) results[0];
				Assert.IsTrue(simple.Date < new DateTime(2005, 01, 01), "should be less than 2005");

				s.Delete("from Simple");
				s.Flush();
			}
		}

		[Test]
		public void CriteriaTypeMismatch()
		{
			using (ISession s = OpenSession())
			{
				Assert.Throws<QueryException>(() =>s.CreateCriteria(typeof(Master))
					.Add(Expression.Like("Details", "SomeString"))
					.List());
			}
		}

		[Test]
		public void CriteriaManyToOneEquals()
		{
			using (ISession s = OpenSession())
			{
				Master master = new Master();
				s.Save(master);
				s.CreateCriteria(typeof(Detail))
					.Add(Expression.Eq("Master", master))
					.List();
				s.Delete(master);
				s.Flush();
			}
		}

		[Test]
		public void CriteriaCompositeProperty()
		{
			using (ISession s = OpenSession())
			{
				Assert.Throws<QueryException>(() =>s.CreateCriteria(typeof(Master))
					.Add(Expression.Eq("Details.I", 10))
					.List());
			}
		}

		[Test]
		public void CriteriaLeftOuterJoin()
		{
			using (ISession s = OpenSession())
			{
				s.Save(new Master());
				s.Flush();
				Assert.AreEqual(1, s.CreateCriteria(typeof(Master))
				                   	.CreateAlias("Details", "detail", JoinType.LeftOuterJoin)
				                   	.Fetch("Details")
				                   	.List().Count);
				s.Delete("from Master");
				s.Flush();
			}
		}

		[Test]
		public void Criteria_can_get_query_entity_type()
		{
			using (ISession s = OpenSession())
			{
				Assert.AreEqual(typeof(Master), 
					s.CreateCriteria(typeof(Master)).GetRootEntityTypeIfAvailable());
			}
		}

		[Test]
		public void DetachedCriteria_can_get_query_entity_type()
		{
			Assert.AreEqual(
				typeof(Master),
				DetachedCriteria.For<Master>().GetRootEntityTypeIfAvailable()
				);
		}

	}
}
