using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2189
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		private Guid _policy2Id;

		protected override void OnSetUp()
		{
			base.OnSetUp();

			using (ISession s = Sfi.OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				TeamMember tm1 = new TeamMember() { Name = "Joe" };
				TeamMember tm2 = new TeamMember() { Name = "Bill" };

				s.Save(tm1);
				s.Save(tm2);

				var policy1 = new Policy() { PolicyNumber = 5 };
				policy1.Tasks.Add(new Task() { Policy = policy1, TaskName = "Task1", TeamMember = tm1 });

				var policy2 = new Policy() { PolicyNumber = 5 };
				policy2.Tasks.Add(new Task() { Policy = policy2, TaskName = "Task2", TeamMember = tm2 });
				policy2.Tasks.Add(new Task() { Policy = policy2, TaskName = "Task3", TeamMember = tm2 });

				s.Save(policy1);
				s.Save(policy2);
				_policy2Id = policy2.Id;

				tx.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (ISession s = Sfi.OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Delete("FROM Task");
				s.Delete("FROM Policy");
				s.Delete("FROM TeamMember");
				tx.Commit();
			}

			base.OnTearDown();
		}

		[Test]
		public void FutureQueryReturnsExistingProxy()
		{
			using (ISession s = Sfi.OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				Policy policyProxy = s.Load<Policy>(_policy2Id);
				Assert.That(NHibernateUtil.IsInitialized(policyProxy), Is.False);

				IEnumerable<Policy> futurePolicy =
					s.CreateQuery("FROM Policy p where p.Id = :id")
						.SetParameter("id", _policy2Id)
						.Future<Policy>();

				Policy queriedPolicy = futurePolicy.ElementAt(0);
				Assert.That(NHibernateUtil.IsInitialized(queriedPolicy));
				Assert.That(queriedPolicy, Is.SameAs(policyProxy));
			}
		}

		[Test]
		public void FutureCriteriaReturnsExistingProxy()
		{
			using (ISession s = Sfi.OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				Policy policyProxy = s.Load<Policy>(_policy2Id);
				Assert.That(NHibernateUtil.IsInitialized(policyProxy), Is.False);

				IEnumerable<Policy> futurePolicy =
					s.CreateCriteria<Policy>()
						.Add(Restrictions.Eq("Id", _policy2Id))
						.Future<Policy>();

				Policy queriedPolicy = futurePolicy.ElementAt(0);
				Assert.That(NHibernateUtil.IsInitialized(queriedPolicy));
				Assert.That(queriedPolicy, Is.SameAs(policyProxy));
			}
		}

		[Test]
		public void FutureQueryEagerLoadUsesAlreadyLoadedEntity()
		{
			using (ISession s = Sfi.OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				Policy policy2 = s.CreateQuery("SELECT p FROM Policy p " +
					"LEFT JOIN FETCH p.Tasks t " +
					"WHERE p.Id = :id")
					.SetParameter("id", _policy2Id)
					.UniqueResult<Policy>();

				Assert.That(NHibernateUtil.IsInitialized(policy2.Tasks));
				Assert.That(NHibernateUtil.IsInitialized(policy2.Tasks.ElementAt(0)));
				Assert.That(NHibernateUtil.IsInitialized(policy2.Tasks.ElementAt(1)));

				IEnumerable<Task> tasks = s.CreateQuery("SELECT t FROM Task t " +
					"INNER JOIN FETCH t.TeamMember ORDER BY t.TaskName")
					.Future<Task>();

				Assert.That(tasks.Count(), Is.EqualTo(3));

				Assert.That(NHibernateUtil.IsInitialized(tasks.ElementAt(0).TeamMember), Is.True, "Task1 TeamMember not initialized");
				Assert.That(NHibernateUtil.IsInitialized(tasks.ElementAt(1).TeamMember), Is.True, "Task2 TeamMember not initialized");
				Assert.That(NHibernateUtil.IsInitialized(tasks.ElementAt(2).TeamMember), Is.True, "Task3 TeamMember not initialized");
			}
		}

		[Test]
		public void FutureCriteriaEagerLoadUsesAlreadyLoadedEntity()
		{
			using (ISession s = Sfi.OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				Policy policy2 =
					s.CreateCriteria<Policy>()
						.Add(Restrictions.Eq("Id", _policy2Id))
						.SetFetchMode("Tasks", FetchMode.Eager)
						.UniqueResult<Policy>();

				Assert.That(NHibernateUtil.IsInitialized(policy2.Tasks));
				Assert.That(NHibernateUtil.IsInitialized(policy2.Tasks.ElementAt(0)));
				Assert.That(NHibernateUtil.IsInitialized(policy2.Tasks.ElementAt(1)));

				IEnumerable<Task> tasks =
					s.CreateCriteria<Task>()
						.SetFetchMode("TeamMember", FetchMode.Eager)
						.AddOrder(Order.Asc("TaskName"))
						.Future<Task>();

				Assert.That(tasks.Count(), Is.EqualTo(3));

				Assert.That(NHibernateUtil.IsInitialized(tasks.ElementAt(0).TeamMember), Is.True, "Task1 TeamMember not initialized");
				Assert.That(NHibernateUtil.IsInitialized(tasks.ElementAt(1).TeamMember), Is.True, "Task2 TeamMember not initialized");
				Assert.That(NHibernateUtil.IsInitialized(tasks.ElementAt(2).TeamMember), Is.True, "Task3 TeamMember not initialized");
			}
		}
	}
}
