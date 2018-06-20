using System.Collections.Generic;
using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.ManyToOneFilters20Behaviour
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		private static IList<Parent> JoinGraphUsingHql(ISession s)
		{
			const string hql = @"select p from Parent p
			                     join p.Child c";
			return s.CreateQuery(hql).List<Parent>();
		}

		private static IList<Parent> JoinGraphUsingCriteria(ISession s)
		{
			return s.CreateCriteria(typeof(Parent)).Fetch("Child").List<Parent>();
		}

		private static Parent CreateParent()
		{
			var ret = new Parent { Child = new Child() };
			ret.Address = new Address { Parent = ret };
			return ret;
		}

		private static void EnableFilters(ISession s)
		{
			var f = s.EnableFilter("activeChild");
			f.SetParameter("active", true);
			var f2 = s.EnableFilter("alwaysValid");
			f2.SetParameter("always", true);
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.Delete("from Parent");
				tx.Commit();
			}
		}

		[Test]
		public void VerifyAlwaysFilter()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var p = CreateParent();
				p.Child.Always = false;
				s.Save(p);
				tx.Commit();
			}

			using (var s = OpenSession())
			{
				EnableFilters(s);
				var resCriteria = JoinGraphUsingCriteria(s);
				var resHql = JoinGraphUsingHql(s);

				Assert.That(resCriteria.Count, Is.EqualTo(0));
				Assert.That(resHql.Count, Is.EqualTo(0));
			}
		}

		[Test]
		public void VerifyFilterActiveButNotUsedInManyToOne()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.Save(CreateParent());
				tx.Commit();
			}

			using (var s = OpenSession())
			{
				EnableFilters(s);
				var resCriteria = JoinGraphUsingCriteria(s);
				var resHql = JoinGraphUsingHql(s);

				Assert.That(resCriteria.Count, Is.EqualTo(1));
				Assert.That(resCriteria[0].Child, Is.Not.Null);

				Assert.That(resHql.Count, Is.EqualTo(1));
				Assert.That(resHql[0].Child, Is.Not.Null);
			}
		}

		[Test]
		public void VerifyQueryWithWhereClause()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var p = CreateParent();
				p.ParentString = "a";
				p.Child.ChildString = "b";
				s.Save(p);
				tx.Commit();
			}
			IList<Parent> resCriteria;
			IList<Parent> resHql;
			using (var s = OpenSession())
			{
				EnableFilters(s);
				resCriteria = s.CreateCriteria(typeof(Parent), "p")
				               .CreateCriteria("Child", "c")
				               .Fetch("Child")
				               .Add(Restrictions.Eq("p.ParentString", "a"))
				               .Add(Restrictions.Eq("c.ChildString", "b"))
				               .List<Parent>();

				resHql = s.CreateQuery(
					          @"select p from Parent p
				                join fetch p.Child c
				                where p.ParentString='a' and c.ChildString='b'")
				          .List<Parent>();
			}
			Assert.That(resCriteria.Count, Is.EqualTo(1));
			Assert.That(resCriteria[0].Child, Is.Not.Null);

			Assert.That(resHql.Count, Is.EqualTo(1));
			Assert.That(resHql[0].Child, Is.Not.Null);
		}

		[Test]
		public void VerifyAlwaysFiltersOnPropertyRef()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var p = CreateParent();
				s.Save(p);
				tx.Commit();
			}

			using (var s = OpenSession())
			{
				EnableFilters(s);
				var resCriteria = JoinGraphUsingCriteria(s);
				var resHql = JoinGraphUsingHql(s);

				Assert.That(resCriteria.Count, Is.EqualTo(1));
				Assert.That(resCriteria[0].Address, Is.Not.Null);
				Assert.That(resHql.Count, Is.EqualTo(1));
				Assert.That(resHql[0].Address, Is.Not.Null);
			}
		}

		[Test]
		public void ExplicitFiltersOnCollectionsShouldBeActive()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var p = CreateParent();
				p.Children = new List<Child>
				{
					new Child {IsActive = true},
					new Child {IsActive = false},
					new Child {IsActive = true}
				};
				s.Save(p);
				tx.Commit();
			}

			using (var s = OpenSession())
			{
				var f = s.EnableFilter("active");
				f.SetParameter("active", true);
				var resCriteria = JoinGraphUsingCriteria(s);
				var resHql = JoinGraphUsingHql(s);

				Assert.That(resCriteria.Count, Is.EqualTo(1));
				Assert.That(resCriteria[0].Children.Count, Is.EqualTo(2));
				Assert.That(resHql.Count, Is.EqualTo(1));
				Assert.That(resHql[0].Children.Count, Is.EqualTo(2));
			}
		}

		[Test]
		public void ExplicitFiltersOnCollectionsShouldBeActiveWithEagerLoad()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var p = CreateParent();
				p.Children = new List<Child>
				{
					new Child {IsActive = true},
					new Child {IsActive = false},
					new Child {IsActive = true}
				};
				s.Save(p);
				tx.Commit();
			}

			using (var s = OpenSession())
			{
				var f = s.EnableFilter("active");
				f.SetParameter("active", true);
				var resCriteria = s.CreateCriteria(typeof(Parent)).Fetch("Children").List<Parent>();
				var resHql = s.CreateQuery("select p from Parent p join fetch p.Children").List<Parent>();

				Assert.That(resCriteria[0].Children.Count, Is.EqualTo(2));
				Assert.That(resHql[0].Children.Count, Is.EqualTo(2));
			}
		}

		[Test]
		public void Verify20BehaviourForPropertyRefAndFilter()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.Save(CreateParent());
				tx.Commit();
			}
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				s.EnableFilter("active")
				 .SetParameter("active", true);

				var resCriteria = s.CreateCriteria(typeof(Parent))
				                   .Fetch("Address")
				                   .List<Parent>();

				var resHql = s.CreateQuery("select p from Parent p join p.Address")
				              .List<Parent>();

				Assert.That(resCriteria.Count, Is.EqualTo(1));
				Assert.That(resCriteria[0].Address, Is.Not.Null);

				Assert.That(resHql.Count, Is.EqualTo(1));
				Assert.That(resHql[0].Address, Is.Not.Null);
			}
		}

	}
}
