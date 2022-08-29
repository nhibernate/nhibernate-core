using System.Collections.Generic;
using System.Linq;
using NHibernate.Collection;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.GenericTest.BagGeneric
{
	[TestFixture]
	public class BagGenericFixture : TestCase
	{
		protected override string[] Mappings
		{
			get { return new string[] { "GenericTest.BagGeneric.BagGenericFixture.hbm.xml" }; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override void OnTearDown()
		{
			using (ISession s = Sfi.OpenSession())
			{
				s.Delete("from A");
				s.Flush();
			}
		}

		[Test]
		public void Simple()
		{
			A a = new A();
			a.Name = "first generic type";
			a.Items = new List<B>();
			B firstB = new B();
			firstB.Name = "first b";
			B secondB = new B();
			secondB.Name = "second b";

			a.Items.Add(firstB);
			a.Items.Add(secondB);

			ISession s = OpenSession();
			s.SaveOrUpdate(a);
			// this flush should test how NH wraps a generic collection with its
			// own persistent collection
			s.Flush();
			s.Close();
			Assert.IsNotNull(a.Id);
			// should have cascaded down to B
			Assert.IsNotNull(firstB.Id);
			Assert.IsNotNull(secondB.Id);

			s = OpenSession();
			a = s.Load<A>(a.Id);
			B thirdB = new B();
			thirdB.Name = "third B";
			// ensuring the correct generic type was constructed
			a.Items.Add(thirdB);
			Assert.AreEqual(3, a.Items.Count, "3 items in the bag now");
			s.Flush();
			s.Close();
		}

		[Test]
		public void EqualsSnapshot()
		{
			var a = new A { Name = "first generic type" };
			var i0 = new B { Name = "1" };
			var i4 = new B { Name = "4" };
			a.Items = new List<B>
			{
				i0,
				i0,
				new B {Name = "2"},
				new B {Name = "3"},
				i4,
				i4,
			};
			var lastIdx = a.Items.Count - 1;
			using (var s = OpenSession())
			{
				s.Save(a);
				s.Flush();
				var collection = (IPersistentCollection) a.Items;
				var collectionPersister = Sfi.GetCollectionPersister(collection.Role);

				a.Items[0] = i4;
				Assert.Multiple(
					() =>
					{
						Assert.That(collection.EqualsSnapshot(collectionPersister), Is.False, "modify first collection element");

						a.Items[lastIdx] = i0;
						Assert.That(collection.EqualsSnapshot(collectionPersister), Is.True, "swap elements in collection");

						a.Items[0] = i0;
						a.Items[lastIdx] = i0;
						Assert.That(collection.EqualsSnapshot(collectionPersister), Is.False, "modify last collection element");

						a.Items[lastIdx] = i4;
						var reversed = a.Items.Reverse().ToArray();
						a.Items.Clear();
						ArrayHelper.AddAll(a.Items, reversed);
						Assert.That(collection.EqualsSnapshot(collectionPersister), Is.True, "reverse collection elements");
					});
			}
		}

		[Test]
		public void Copy()
		{
			A a = new A();
			a.Name = "original A";
			a.Items = new List<B>();

			B b1 = new B();
			b1.Name = "b1";
			a.Items.Add(b1);

			B b2 = new B();
			b2.Name = "b2";
			a.Items.Add(b2);

			A copiedA;
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				copiedA = s.Merge(a);
				t.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				A loadedA = s.Get<A>(copiedA.Id);
				Assert.IsNotNull(loadedA);
				s.Delete(loadedA);
				t.Commit();
			}
		}
	}
}
