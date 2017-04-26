using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2603
{
	public class Fixture : BugTestCase
	{
		#region Scenarios

		private class ListScenario : IDisposable
		{
			private readonly ISessionFactory factory;

			public ListScenario(ISessionFactory factory)
			{
				this.factory = factory;
				using (ISession s = factory.OpenSession())
				using (ITransaction t = s.BeginTransaction())
				{
					var entity = new Parent();
					var child = new Child();
					entity.ListChildren = new List<Child> {null, child, null};
					s.Save(entity);
					t.Commit();
				}
			}

			public void Dispose()
			{
				using (ISession s = factory.OpenSession())
				using (ITransaction t = s.BeginTransaction())
				{
					s.Delete("from Parent");
					s.Delete("from Child");
					t.Commit();
				}
			}
		}

		private class MapScenario : IDisposable
		{
			private readonly ISessionFactory factory;

			public MapScenario(ISessionFactory factory)
			{
				this.factory = factory;
				using (ISession s = factory.OpenSession())
				using (ITransaction t = s.BeginTransaction())
				{
					var entity = new Parent();
					entity.MapChildren = new Dictionary<int, Child>
					{
						{0, null},
						{1, new Child()},
						{2, null},
					};
					s.Save(entity);
					t.Commit();
				}
			}

			public void Dispose()
			{
				using (ISession s = factory.OpenSession())
				using (ITransaction t = s.BeginTransaction())
				{
					s.Delete("from Parent");
					s.Delete("from Child");
					t.Commit();
				}
			}
		}

		#endregion

		[Test]
		public void List()
		{
			using (new ListScenario(Sfi))
			{
				// by design NH will clean null elements at the end of the List since 'null' and 'no element' mean the same.
				// the effective ammount store will be 1(one) because ther is only one valid element but whem we initialize the collection
				// it will have 2 elements (the first with null)
				using (ISession s = OpenSession())
				using (ITransaction t = s.BeginTransaction())
				{
					var entity = s.CreateQuery("from Parent").UniqueResult<Parent>();
					IList<object[]> members = s.GetNamedQuery("ListMemberSpy")
											   .SetParameter("parentid", entity.Id)
											   .List<object[]>();
					int lazyCount = entity.ListChildren.Count;
					Assert.That(NHibernateUtil.IsInitialized(entity.ListChildren), Is.False);
					NHibernateUtil.Initialize(entity.ListChildren);
					int initCount = entity.ListChildren.Count;
					Assert.That(initCount, Is.EqualTo(lazyCount));
					Assert.That(members, Has.Count.EqualTo(1), "because only the valid element should be persisted.");
				}
			}
		}

		[Test]
		public void Map()
		{
			using (new MapScenario(Sfi))
			using (ISession s = OpenSession())
			{
				// for the case of <map> what really matter is the key, then NH should count the KEY and not the elements.
				using (ITransaction t = s.BeginTransaction())
				{
					var entity = s.CreateQuery("from Parent").UniqueResult<Parent>();
					IList<object[]> members = s.GetNamedQuery("MapMemberSpy")
											   .SetParameter("parentid", entity.Id)
											   .List<object[]>();
					int lazyCount = entity.MapChildren.Count;
					Assert.That(NHibernateUtil.IsInitialized(entity.MapChildren), Is.False);
					NHibernateUtil.Initialize(entity.MapChildren);
					int initCount = entity.MapChildren.Count;
					Assert.That(initCount, Is.EqualTo(lazyCount));
					Assert.That(members, Has.Count.EqualTo(3), "because all elements with a valid key should be persisted.");
				}
			}
		}
	}
}