using System.Collections.Generic;
using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.ManyToOneFilters20Behaviour
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		private static IList<Parent> joinGraphUsingHql(ISession s)
		{
			const string hql = @"select p from Parent p
                                    join p.Child c";
			return s.CreateQuery(hql).List<Parent>();
		}

		private static IList<Parent> joinGraphUsingCriteria(ISession s)
		{
			return s.CreateCriteria(typeof(Parent)).SetFetchMode("Child", FetchMode.Join).List<Parent>();
		}

		private static Parent createParent()
		{
			var ret = new Parent { Child = new Child() };
			ret.Address = new Address { Parent = ret };
			return ret;
		}

		private static void enableFilters(ISession s)
		{
			IFilter f = s.EnableFilter("activeChild");
			f.SetParameter("active", true);
			IFilter f2 = s.EnableFilter("alwaysValid");
			f2.SetParameter("always", true);
		}

		protected override void OnTearDown()
		{
			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					s.Delete("from Parent");
					tx.Commit();
				}
			}
		}

		[Test]
		public void VerifyAlwaysFilter()
		{
			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					Parent p = createParent();
					p.Child.Always = false;
					s.Save(p);
					tx.Commit();
				}
			}

			using (ISession s = OpenSession())
			{
				enableFilters(s);
				IList<Parent> resCriteria = joinGraphUsingCriteria(s);
				IList<Parent> resHql = joinGraphUsingHql(s);

				Assert.AreEqual(0, resCriteria.Count);
				Assert.AreEqual(0, resHql.Count);
			}
		}

		[Test]
		public void VerifyFilterActiveButNotUsedInManyToOne()
		{
			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					s.Save(createParent());
					tx.Commit();
				}
			}

			using (ISession s = OpenSession())
			{
				enableFilters(s);
				IList<Parent> resCriteria = joinGraphUsingCriteria(s);
				IList<Parent> resHql = joinGraphUsingHql(s);

				Assert.AreEqual(1, resCriteria.Count);
				Assert.IsNotNull(resCriteria[0].Child);

				Assert.AreEqual(1, resHql.Count);
				Assert.IsNotNull(resHql[0].Child);
			}
		}

		[Test]
		public void VerifyQueryWithWhereClause()
		{
			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					var p = createParent();
					p.ParentString = "a";
					p.Child.ChildString = "b";
					s.Save(p);
					tx.Commit();
				}
			}
			IList<Parent> resCriteria;
			IList<Parent> resHql;
			using (ISession s = OpenSession())
			{
				enableFilters(s);
				resCriteria = s.CreateCriteria(typeof(Parent), "p")
												.CreateCriteria("Child", "c")
												.SetFetchMode("Child", FetchMode.Join)
												.Add(Restrictions.Eq("p.ParentString", "a"))
												.Add(Restrictions.Eq("c.ChildString", "b"))
												.List<Parent>();
				resHql = s.CreateQuery(@"select p from Parent p
                                                        join fetch p.Child c
                                                    where p.ParentString='a' and c.ChildString='b'").List<Parent>();
			}
			Assert.AreEqual(1, resCriteria.Count);
			Assert.IsNotNull(resCriteria[0].Child);
			Assert.AreEqual(1, resHql.Count);
			Assert.IsNotNull(resHql[0].Child);
		}

		[Test]
		public void VerifyAlwaysFiltersOnPropertyRef()
		{
			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					Parent p = createParent();
					s.Save(p);
					tx.Commit();
				}
			}

			using (ISession s = OpenSession())
			{
				enableFilters(s);
				IList<Parent> resCriteria = joinGraphUsingCriteria(s);
				IList<Parent> resHql = joinGraphUsingHql(s);

				Assert.IsNotNull(resCriteria[0].Address);
				Assert.IsNotNull(resHql[0].Address);
			}
		}

		[Test]
		public void ExplicitFiltersOnCollectionsShouldBeActive()
		{
			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					Parent p = createParent();
					p.Children = new List<Child>
                                     {
                                         new Child {IsActive = true},
                                         new Child {IsActive = false},
                                         new Child {IsActive = true}
                                     };
					s.Save(p);
					tx.Commit();
				}
			}

			using (ISession s = OpenSession())
			{
				IFilter f = s.EnableFilter("active");
				f.SetParameter("active", true);
				IList<Parent> resCriteria = joinGraphUsingCriteria(s);
				IList<Parent> resHql = joinGraphUsingHql(s);

				Assert.AreEqual(2, resCriteria[0].Children.Count);
				Assert.AreEqual(2, resHql[0].Children.Count);
			}
		}

		[Test]
		public void ExplicitFiltersOnCollectionsShouldBeActiveWithEagerLoad()
		{
			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					Parent p = createParent();
					p.Children = new List<Child>
                                     {
                                         new Child {IsActive = true},
                                         new Child {IsActive = false},
                                         new Child {IsActive = true}
                                     };
					s.Save(p);
					tx.Commit();
				}
			}

			using (ISession s = OpenSession())
			{
				IFilter f = s.EnableFilter("active");
				f.SetParameter("active", true);
				IList<Parent> resCriteria = s.CreateCriteria(typeof(Parent)).SetFetchMode("Children", FetchMode.Join).List<Parent>();
				IList<Parent> resHql = s.CreateQuery("select p from Parent p join fetch p.Children").List<Parent>();

				Assert.AreEqual(2, resCriteria[0].Children.Count);
				Assert.AreEqual(2, resHql[0].Children.Count);
			}
		}
	}
}
