using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2489
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
				{
					using (ITransaction t = s.BeginTransaction())
					{
						var entity = new Base();
						var child = new Child();
						// null members are partially working for lists, can't have one at the end
						// and can't use the Count property.
						entity.Children = new List<Child> {null, child};
						s.Save(entity);
						t.Commit();
					}
				}
			}

			public void Dispose()
			{
				using (ISession s = factory.OpenSession())
				{
					using (ITransaction t = s.BeginTransaction())
					{
						s.Delete("from Base");
						s.Delete("from Child");
						t.Commit();
					}
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
				{
					using (ITransaction t = s.BeginTransaction())
					{
						var entity = new Base();
						entity.NamedChildren = new Dictionary<string, Child>
											   {
												{"Child1", new Child()},
												{"NullChild", null},
											   };

						var child1 = new AnotherChild { Name = "AnotherChild1" };
						var child2 = new AnotherChild { Name = "AnotherChild2" };

						s.Save(child1);
						s.Save(child2);

						entity.OneToManyNamedChildren = new Dictionary<string, AnotherChild> 
											   {
												{"AnotherChild1" , child1}, 
												{"AnotherChild2" , child2} 
											   };

						s.Save(entity);
						t.Commit();
					}
				}
			}

			public void Dispose()
			{
				using (ISession s = factory.OpenSession())
				{
					using (ITransaction t = s.BeginTransaction())
					{
						s.Delete("from Base");
						s.Delete("from Child");
						t.Commit();
					}
				}
			}
		}

		#endregion

		[Test]
		public void List_InvalidIndex()
		{
			using (new ListScenario(Sfi))
			{
				using (ISession s = OpenSession())
				{
					using (ITransaction t = s.BeginTransaction())
					{
						// accessing an invalid index should throw an exception
						var entity = s.CreateQuery("from Base").UniqueResult<Base>();
						// null collection members don't seem to work, at least for lazy="extra" collections
						Assert.That(entity.Children.Count, Is.EqualTo(2));
						Assert.That(NHibernateUtil.IsInitialized(entity.Children), Is.False);
						Assert.That(() => { Child ignored = entity.Children[2]; }, Throws.TypeOf<ArgumentOutOfRangeException>());
					}
				}
			}
		}

		[Test]
		public void List_NullChild()
		{
			using (new ListScenario(Sfi))
			{
				using (ISession s = OpenSession())
				{
					using (ITransaction t = s.BeginTransaction())
					{
						// the list should only contain an item at index 0
						// accessing an invalid index should throw an exception
						var entity = s.CreateQuery("from Base").UniqueResult<Base>();
						// null collection members don't seem to work, at least for lazy="extra" collections
						Assert.That(entity.Children.Count, Is.Not.EqualTo(0));
						//entity.Children.Count.Should().Be.EqualTo(2);
						Assert.That(NHibernateUtil.IsInitialized(entity.Children), Is.False);
						var sigil = new Child();
						Child child = sigil;
						Assert.That(() => { child = entity.Children[0]; }, Throws.Nothing);
						Assert.That(child, Is.Not.EqualTo(sigil));
						Assert.That(child, Is.Null);
					}
				}
			}
		}

		[Test]
		public void Map_Item()
		{
			using (new MapScenario(Sfi))
			{
				using (ISession s = OpenSession())
				{
					using (ITransaction t = s.BeginTransaction())
					{
						// accessing an invalid key should fail or throw an exception, depending on method
						var entity = s.CreateQuery("from Base").UniqueResult<Base>();
						// null collection members don't seem to work, at least for lazy="extra" collections
						Assert.That(entity.NamedChildren.Count, Is.EqualTo(2));
						Assert.That(entity.OneToManyNamedChildren.Count, Is.EqualTo(2));
						Assert.That(NHibernateUtil.IsInitialized(entity.NamedChildren), Is.False);
						Assert.That(() => { Child ignored = entity.NamedChildren["InvalidKey"]; }, Throws.TypeOf<KeyNotFoundException>());
						Assert.That(() => { AnotherChild ignored = entity.OneToManyNamedChildren["InvalidKey"]; }, Throws.TypeOf<KeyNotFoundException>());
						Assert.That(NHibernateUtil.IsInitialized(entity.NamedChildren), Is.False);
					}
				}
			}
		}

		[Test]
		public void Map_TryGetValue_Invalid()
		{
			using (new MapScenario(Sfi))
			{
				using (ISession s = OpenSession())
				{
					using (ITransaction t = s.BeginTransaction())
					{
						// accessing an invalid key should fail or throw an exception, depending on method
						var entity = s.CreateQuery("from Base").UniqueResult<Base>();
						// null collection members don't seem to work, at least for lazy="extra" collections
						Assert.That(entity.NamedChildren.Count, Is.EqualTo(2));
						Assert.That(NHibernateUtil.IsInitialized(entity.NamedChildren), Is.False);
						Child child;
						Assert.That(entity.NamedChildren.TryGetValue("InvalidKey", out child), Is.False);
						Assert.That(child, Is.Null);
						AnotherChild anotherChild;
						Assert.That(entity.OneToManyNamedChildren.TryGetValue("InvalidKey", out anotherChild), Is.False);
						Assert.That(child, Is.Null);
						Assert.That(NHibernateUtil.IsInitialized(entity.NamedChildren), Is.False);
					}
				}
			}
		}

		[Test]
		public void Map_NullChild()
		{
			using (new MapScenario(Sfi))
			{
				using (ISession s = OpenSession())
				{
					using (ITransaction t = s.BeginTransaction())
					{
						var entity = s.CreateQuery("from Base").UniqueResult<Base>();
						// null collection members don't seem to work, at least for lazy="extra" collections
						Assert.That(entity.NamedChildren.Count, Is.Not.EqualTo(0));
						//entity.NamedChildren.Count.Should().Be.EqualTo(2);
						Assert.That(NHibernateUtil.IsInitialized(entity.NamedChildren), Is.False);
						// null valued child shouldn't cause errors
						var sigil = new Child();
						Child child = sigil;
						Assert.DoesNotThrow(() => { child = entity.NamedChildren["NullChild"]; });
						Assert.That(child, Is.Not.EqualTo(sigil));
						Assert.That(child, Is.Null);
					}
				}
			}
		}

		[Test]
		public void Map_NullChild_TryGetValue()
		{
			using (new MapScenario(Sfi))
			{
				using (ISession s = OpenSession())
				{
					using (ITransaction t = s.BeginTransaction())
					{
						var entity = s.CreateQuery("from Base").UniqueResult<Base>();
						// null collection members don't seem to work, at least for lazy="extra" collections
						Assert.That(entity.NamedChildren.Count, Is.Not.EqualTo(0));
						//entity.NamedChildren.Count.Should().Be.EqualTo(2);
						// null valued child shouldn't cause errors
						Assert.That(NHibernateUtil.IsInitialized(entity.NamedChildren), Is.False);
						Child child;
						Assert.That(entity.NamedChildren.TryGetValue("NullChild", out child), Is.True);
						Assert.That(child, Is.Null);
					}
				}
			}
		}
	}
}