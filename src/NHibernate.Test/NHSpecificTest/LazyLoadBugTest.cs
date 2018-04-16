using System;
using System.Collections;
using NHibernate.DomainModel.NHSpecific;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest
{
	[TestFixture]
	public class LazyLoadBugTest : TestCase
	{
		protected override IList Mappings
		{
			get { return new string[] {"NHSpecific.LazyLoadBug.hbm.xml"}; }
		}

		[Test]
		public void TestLazyLoad()
		{
			int parentId = 0;

			using (ISession s1 = OpenSession())
			using (ITransaction t1 = s1.BeginTransaction())
			{
				// create a new
				LLParent parent = new LLParent();
				LLChild child = new LLChild();
				child.Parent = parent;

				s1.Save(parent);
				parentId = (int) s1.GetIdentifier(parent);

				t1.Commit();
			}

			try
			{
				// try to Load the object to get the exception
				using (ISession s2 = OpenSession())
				using (ITransaction t2 = s2.BeginTransaction())
				{
					LLParent parent2 = (LLParent) s2.Load(typeof(LLParent), parentId);

					// this should throw the exception - the property setter access is not mapped correctly.
					// Because it maintains logic to maintain the collection during the property set it should
					// tell NHibernate to skip the setter and access the field.  If it doesn't, then throw
					// a LazyInitializationException.
					Assert.Throws<LazyInitializationException>(() => { int count = parent2.Children.Count; });
				}
			}
			finally
			{
				// Need to delete the objects using direct SQL in this case,
				// because of loading problems.
				ExecuteStatement("delete from LLChild");
				ExecuteStatement("delete from LLParent");
			}
		}

		[Test]
		public void TestLazyLoadNoAdd()
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

			using (ISession session = Sfi.OpenSession())
			{
				session.Delete("from LLParent");
				session.Flush();
			}
		}
	}
}