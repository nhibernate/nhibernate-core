using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Test.MultipleCollectionFetchTest
{
	public abstract class AbstractMultipleCollectionFetchFixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected abstract void AddToCollection(ICollection<Person> collection, Person person);

		protected abstract ICollection<Person> CreateCollection();

		protected override void OnTearDown()
		{
			base.OnTearDown();
			using (ISession s = OpenSession())
			{
				s.Delete("from Person p where p.Parent is null");
				s.Flush();
			}
		}

		private Person CreateGrandparent()
		{
			Person parent = new Person();
			parent.Children = CreateCollection();

			for (int i = 0; i < 2; i++)
			{
				Person child = new Person();
				child.Parent = parent;
				AddToCollection(parent.Children, child);

				child.Children = CreateCollection();

				for (int j = 0; j < 3; j++)
				{
					Person grandChild = new Person();
					grandChild.Parent = child;
					AddToCollection(child.Children, grandChild);
				}
			}

			return parent;
		}

		protected virtual void RunLinearJoinFetchTest(Person parent)
		{
			using (ISession s = OpenSession())
			using(ITransaction tx = s.BeginTransaction())
			{
				s.Save(parent);
				tx.Commit();
			}
			using (ISession s = OpenSession())
			{
				Person p = (Person) s.CreateQuery(
										"select p from Person p join fetch p.Children c join fetch c.Children gc")
										.UniqueResult();

				Assert.IsTrue(NHibernateUtil.IsInitialized(p.Children));
				Assert.AreEqual(2, p.Children.Count);
				foreach (Person child in p.Children)
				{
					Assert.IsTrue(NHibernateUtil.IsInitialized(child));
					Assert.IsTrue(NHibernateUtil.IsInitialized(child.Children));
					Assert.AreEqual(3, child.Children.Count);
				}
			}
		}

		// Tests "linear" join fetch, i.e. A join A.B join A.B.C
		[Test]
		public void MultipleCollectionsLinearJoinFetch()
		{
			Person parent = CreateGrandparent();
			RunLinearJoinFetchTest(parent);
		}

		private Person CreateParentAndFriend()
		{
			Person parent = new Person();
			parent.Children = CreateCollection();

			for (int i = 0; i < 2; i++)
			{
				Person child = new Person();
				child.Parent = parent;
				AddToCollection(parent.Children, child);
			}

			parent.Friends = CreateCollection();

			for (int i = 0; i < 3; i++)
			{
				Person friend = new Person();
				AddToCollection(parent.Friends, friend);
			}

			return parent;
		}

		protected virtual void RunNonLinearJoinFetchTest(Person person)
		{
			using (ISession s = OpenSession())
			{
				s.Save(person);
				s.Flush();
			}
			try
			{
				using (ISession s = OpenSession())
				{
					Person p = (Person) s.CreateQuery(
											"select p from Person p join fetch p.Children join fetch p.Friends f")
											.UniqueResult();

					Assert.IsTrue(NHibernateUtil.IsInitialized(p.Children));
					Assert.AreEqual(2, p.Children.Count);
					foreach (Person child in p.Children)
					{
						Assert.IsTrue(NHibernateUtil.IsInitialized(child));
					}
					Assert.IsTrue(NHibernateUtil.IsInitialized(p.Friends));
					Assert.AreEqual(3, p.Friends.Count);
					foreach (Person friend in p.Friends)
					{
						Assert.IsTrue(NHibernateUtil.IsInitialized(friend));
					}
				}
			}
			finally
			{
				using (ISession s = OpenSession())
				{
					s.Delete("from Person p where p.Parent is null");
					s.Flush();
				}
			}
		}

		// Tests "non-linear" join fetch, i.e. A join A.B join A.C
		[Test]
		public void MultipleCollectionsNonLinearJoinFetch()
		{
			Person person = CreateParentAndFriend();
			RunNonLinearJoinFetchTest(person);
		}
	}
}
