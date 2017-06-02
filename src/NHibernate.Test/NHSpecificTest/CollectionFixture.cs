using System;
using System.Collections;
using NHibernate.DomainModel.NHSpecific;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest
{
	/// <summary>
	/// Tests loading of collections very simply.
	/// </summary>
	[TestFixture]
	public class CollectionFixture : TestCase
	{
		protected override IList Mappings
		{
			get { return new string[] {"NHSpecific.LazyLoadBug.hbm.xml"}; }
		}

		protected override void OnTearDown()
		{
			using (ISession session = Sfi.OpenSession())
			{
				session.Delete("from LLParent");
				session.Flush();
			}
		}

		[Test]
		public void TestLoadParentFirst()
		{
			int parentId = 0;

			using (ISession s1 = OpenSession())
			using (ITransaction t1 = s1.BeginTransaction())
			{
				// create a new
				LLParent parent = new LLParent();
				LLChildNoAdd child = new LLChildNoAdd();
				parent.ChildrenNoAdd.Add(child);
				child.Parent = parent;

				s1.Save(parent);
				parentId = (int) s1.GetIdentifier(parent);

				t1.Commit();
			}

			// try to Load the object to make sure the save worked
			using (ISession s2 = OpenSession())
			using (ITransaction t2 = s2.BeginTransaction())
			{
				LLParent parent2 = (LLParent) s2.Load(typeof(LLParent), parentId);
				Assert.AreEqual(1, parent2.ChildrenNoAdd.Count);
			}
		}

		[Test]
		public void TestLoadChildFirst()
		{
			int parentId = 0;
			int childId = 0;

			using (ISession s1 = OpenSession())
			using (ITransaction t1 = s1.BeginTransaction())
			{
				// create a new
				LLParent parent = new LLParent();
				LLChildNoAdd child = new LLChildNoAdd();
				parent.ChildrenNoAdd.Add(child);
				child.Parent = parent;

				s1.Save(parent);
				parentId = (int) s1.GetIdentifier(parent);
				childId = (int) s1.GetIdentifier(child);

				t1.Commit();
			}

			// try to Load the object to make sure the save worked
			using (ISession s2 = OpenSession())
			using (ITransaction t2 = s2.BeginTransaction())
			{
				LLChildNoAdd child2 = (LLChildNoAdd) s2.Load(typeof(LLChildNoAdd), childId);
				Assert.AreEqual(parentId, (int) s2.GetIdentifier(child2.Parent));
			}
		}
	}
}