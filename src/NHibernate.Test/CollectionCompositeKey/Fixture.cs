using System;
using System.Collections.Generic;
using NHibernate.Event;
using NHibernate.Persister.Collection;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace NHibernate.Test.CollectionCompositeKey
{
	[TestFixture]
	public class Fixture : TestCase
	{
		private readonly Parent _parentId = new Parent("1", 1);
		private int _currentChildId = 1;
		private int _currentGrandChildId = 1;

		protected override string[] Mappings => new [] { "CollectionCompositeKey.CollectionOwner.hbm.xml" };

		protected override string MappingsAssembly => "NHibernate.Test";

		protected override string CacheConcurrencyStrategy => null;

		protected override void OnSetUp()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var parent = CreateParent(_parentId.Code, _parentId.Number);

				s.Save(parent);
				t.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				session.Delete("from GrandChild");
				session.Delete("from Child");
				session.Delete("from Parent");
				tx.Commit();
			}
			base.OnTearDown();
		}

		[TestCase(nameof(Parent.Children), true, true)]
		[TestCase(nameof(Parent.ChildrenByForeignKeys), true, false)]
		[TestCase(nameof(Parent.ChildrenByComponent), true, true)]
		[TestCase(nameof(Parent.ChildrenByComponentForeignKeys), true, false)]
		[TestCase(nameof(Parent.ChildrenNoProperties), false, false)]
		[TestCase(nameof(Parent.ChildrenByUniqueKey), true, true)]
		[TestCase(nameof(Parent.ChildrenByUniqueKeyNoManyToOne), true, false)]
		[TestCase(nameof(Parent.ChildrenByCompositeUniqueKey), true, true)]
		[TestCase(nameof(Parent.ChildrenByCompositeUniqueKeyNoManyToOne), true, false)]
		public void TestGetElementOwnerForChildCollections(string collectionProperty, bool propertyExists, bool hasManyToOne)
		{
			TestGetElementOwnerForChildCollections<Child>(collectionProperty, propertyExists, hasManyToOne, (parent, child) => child.Id);
		}

		[TestCase(nameof(Parent.GrandChildren), true, true)]
		public void TestGetElementOwnerForGrandChildCollections(string collectionProperty, bool propertyExists, bool hasManyToOne)
		{
			TestGetElementOwnerForChildCollections<GrandChild>(collectionProperty, propertyExists, hasManyToOne, GetGrandChildId);
		}

		private static object GetGrandChildId(Parent parent, GrandChild child)
		{
			if (parent == null)
			{
				return null; // Not supported
			}

			child.GrandParent = parent;
			return child;
		}

		private void TestGetElementOwnerForChildCollections<TChild>(string collectionProperty, bool propertyExists, bool hasManyToOne, Func<Parent, TChild, object> getChildId)
			where TChild : class
		{
			var persister = Sfi.GetEntityPersister(typeof(Parent).FullName);
			var collPersister = GetCollectionPersister(collectionProperty);
			TChild firstChild = null;

			var propertyIndex = -1;
			for (var i = 0; i < persister.PropertyNames.Length; i++)
			{
				if (persister.PropertyNames[i] == collectionProperty)
				{
					propertyIndex = i;
					break;
				}
			}

			Assert.That(propertyIndex, Is.Not.EqualTo(-1));

			// Test when collection is loaded
			using (var s = (IEventSource) OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var parent = s.Get<Parent>(_parentId);
				Assert.That(parent, Is.Not.Null);

				var collection = (IList<TChild>) persister.GetPropertyValue(parent, propertyIndex);
				foreach (var child in collection)
				{
					if (firstChild == null)
					{
						firstChild = child;
					}

					Assert.That(collPersister.GetElementOwner(child, s), propertyExists ? Is.EqualTo(parent) : (IResolveConstraint) Is.Null);
				}

				tx.Commit();
			}

			Assert.That(firstChild, Is.Not.Null);

			// Test when collection is not loaded
			using (var s = (IEventSource) OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var parent = s.Get<Parent>(_parentId);
				var child = s.Get<TChild>(getChildId(parent, firstChild));
				Assert.That(parent, Is.Not.Null);
				Assert.That(child, Is.Not.Null);

				Assert.That(collPersister.GetElementOwner(child, s), propertyExists ? Is.EqualTo(parent) : (IResolveConstraint) Is.Null);

				tx.Commit();
			}

			// Test when only the child is loaded
			using (var s = (IEventSource) OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var id = getChildId(null, firstChild);
				if (id != null)
				{
					var child = s.Get<TChild>(id);
					Assert.That(child, Is.Not.Null);

					Assert.That(collPersister.GetElementOwner(child, s), hasManyToOne ? Is.InstanceOf<Parent>() : (IResolveConstraint) Is.Null);
				}

				tx.Commit();
			}

			// Test transient
			using (var s = (IEventSource) OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var parent = CreateParent("2", 2);
				var collection = (IList<TChild>) persister.GetPropertyValue(parent, propertyIndex);

				foreach (var child in collection)
				{
					Assert.That(collPersister.GetElementOwner(child, s), hasManyToOne ? Is.EqualTo(parent) : (IResolveConstraint) Is.Null);
				}

				tx.Commit();
			}
		}

		private AbstractCollectionPersister GetCollectionPersister(string collectionProperty)
		{
			return (AbstractCollectionPersister) Sfi.GetCollectionPersister($"{typeof(Parent).FullName}.{collectionProperty}");
		}

		private Parent CreateParent(string code, int number)
		{
			var parent = new Parent(code, number)
			{
				Name = $"parent{number}",
				ReferenceCode = code,
				ReferenceNumber = number
			};

			parent.Children.Add(new Child(_currentChildId++, "child", parent) { Parent = parent });
			parent.ChildrenByForeignKeys.Add(new Child(_currentChildId++, "childFk", parent) { ParentNumber = parent.Number, ParentCode = parent.Code });
			parent.ChildrenByComponent.Add(new Child(_currentChildId++, "childCo", parent) { Component = new ChildComponent { Parent = parent } });
			parent.ChildrenByComponentForeignKeys.Add(
				new Child(_currentChildId++, "childCoFk", parent)
				{
					Component = new ChildComponent { ParentNumber = parent.Number, ParentCode = parent.Code }
				});
			parent.ChildrenNoProperties.Add(new Child(_currentChildId++, "childNp", parent));
			parent.ChildrenByUniqueKey.Add(new Child(_currentChildId++, "childUk", parent) { ParentByName = parent });
			parent.ChildrenByUniqueKeyNoManyToOne.Add(new Child(_currentChildId++, "childUkFk", parent) { ParentName = parent.Name });
			parent.ChildrenByCompositeUniqueKey.Add(new Child(_currentChildId++, "childCoUk", parent) { ParentByReference = parent });
			parent.ChildrenByCompositeUniqueKeyNoManyToOne.Add(
				new Child(_currentChildId++, "childCoUkFk", parent)
				{
					ParentReferenceCode = parent.ReferenceCode,
					ParentReferenceNumber = parent.ReferenceNumber
				});

			parent.GrandChildren.Add(new GrandChild(_currentGrandChildId, "grandChild", parent));

			return parent;
		}
	}
}
