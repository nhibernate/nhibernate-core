﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Collections;
using NHibernate.DomainModel;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NUnit.Framework;

namespace NHibernate.Test.Legacy
{
	using System.Threading.Tasks;
	[TestFixture]
	public class CriteriaTestAsync : TestCase
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
		public async Task SimpleSelectTestAsync()
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
				await (s1.SaveAsync(notSimple1, notSimple1Key));
				await (s1.SaveAsync(simple1, simple1Key));
				await (t1.CommitAsync());
			}

			using (ISession s2 = OpenSession())
			using (ITransaction t2 = s2.BeginTransaction())
			{
				var results2 = await (s2.CreateCriteria<Simple>()
					.Add(Restrictions.Eq("Address", "Street 12"))
					.ListAsync<Simple>());

				Assert.That(results2.Count, Is.EqualTo(1), "Unexpected result count");

				var simple2 = results2[0];

				Assert.That(simple2, Is.Not.Null, "Unable to load object");
				Assert.That(simple2.Count, Is.EqualTo(simple1.Count), "Unexpected Count property value");
				Assert.That(simple2.Name, Is.EqualTo(simple1.Name), "Unexpected name");
				Assert.That(simple2.Address, Is.EqualTo(simple1.Address), "Unexpected address");
				Assert.That(simple2.Date, Is.EqualTo(simple1.Date), "Unexpected date");

				await (s2.DeleteAsync("from Simple"));

				await (t2.CommitAsync());
			}
		}

		[Test]
		public async Task SimpleDateCriteriaAsync()
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
				await (s.SaveAsync(s1, 1L));
				await (s.SaveAsync(s2, 2L));
				await (s.FlushAsync());
			}

			using (ISession s = OpenSession())
			{
				IList results = await (s.CreateCriteria(typeof(Simple))
					.Add(Expression.Gt("Date", new DateTime(2005, 01, 01)))
					.AddOrder(Order.Asc("Date"))
					.ListAsync());

				Assert.AreEqual(1, results.Count, "one gt from 2005");
				Simple simple = (Simple) results[0];
				Assert.IsTrue(simple.Date > new DateTime(2005, 01, 01), "should have returned dates after 2005");

				results = await (s.CreateCriteria(typeof(Simple))
					.Add(Expression.Lt("Date", new DateTime(2005, 01, 01)))
					.AddOrder(Order.Asc("Date"))
					.ListAsync());

				Assert.AreEqual(1, results.Count, "one lt than 2005");
				simple = (Simple) results[0];
				Assert.IsTrue(simple.Date < new DateTime(2005, 01, 01), "should be less than 2005");

				await (s.DeleteAsync("from Simple"));
				await (s.FlushAsync());
			}
		}

		[Test]
		public void CriteriaTypeMismatchAsync()
		{
			using (ISession s = OpenSession())
			{
				Assert.ThrowsAsync<QueryException>(() =>s.CreateCriteria(typeof(Master))
					.Add(Expression.Like("Details", "SomeString"))
					.ListAsync());
			}
		}

		[Test]
		public async Task CriteriaManyToOneEqualsAsync()
		{
			using (ISession s = OpenSession())
			{
				Master master = new Master();
				await (s.SaveAsync(master));
				await (s.CreateCriteria(typeof(Detail))
					.Add(Expression.Eq("Master", master))
					.ListAsync());
				await (s.DeleteAsync(master));
				await (s.FlushAsync());
			}
		}

		[Test]
		public void CriteriaCompositePropertyAsync()
		{
			using (ISession s = OpenSession())
			{
				Assert.ThrowsAsync<QueryException>(() =>s.CreateCriteria(typeof(Master))
					.Add(Expression.Eq("Details.I", 10))
					.ListAsync());
			}
		}

		[Test]
		public async Task CriteriaLeftOuterJoinAsync()
		{
			using (ISession s = OpenSession())
			{
				await (s.SaveAsync(new Master()));
				await (s.FlushAsync());
				Assert.AreEqual(1, (await (s.CreateCriteria(typeof(Master))
				                   	.CreateAlias("Details", "detail", JoinType.LeftOuterJoin)
				                   	.Fetch("Details")
				                   	.ListAsync())).Count);
				await (s.DeleteAsync("from Master"));
				await (s.FlushAsync());
			}
		}

	}
}
