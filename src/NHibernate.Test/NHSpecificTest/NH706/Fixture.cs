using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH706
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		public void Bug()
		{
			Parent parent = new Parent();
			parent.Name = "Parent";

			RelatedObject obj1 = new RelatedObject();
			obj1.Name = "Related Object 1";

			RelatedObject obj2 = new RelatedObject();
			obj2.Name = "Related Object 2";

			Child child1 = new Child();
			child1.Parent = parent;
			child1.RelatedObject = obj1;
			child1.Name = "Child 1";

			Child child2 = new Child();
			child2.Parent = parent;
			child2.RelatedObject = obj2;
			child2.Name = "Child 2";

			parent.Children = new SortedSet<Child>(new ChildComparer());
			parent.Children.Add(child1);
			parent.Children.Add(child2);

			DifferentChild dc1 = new DifferentChild();
			dc1.Parent = parent;
			dc1.Name = "Different Child 1";
			dc1.Child = child1;

			DifferentChild dc2 = new DifferentChild();
			dc2.Parent = parent;
			dc2.Name = "Different Child 2";
			dc2.Child = child2;

			parent.DifferentChildren = new HashSet<DifferentChild>();
			parent.DifferentChildren.Add(dc1);
			parent.DifferentChildren.Add(dc2);
			using (ISession session = OpenSession())
			{
				session.Save(obj1);
				session.Save(obj2);

				session.Save(parent);

				session.Save(child1);
				session.Save(child2);

				session.Save(dc1);
				session.Save(dc2);
				session.Flush();
			}

			int dcId = 0;
			using (ISession session = OpenSession())
			{
				Parent loadedParent = (Parent) session.Get(typeof(Parent), parent.ID);
				NHibernateUtil.Initialize(loadedParent.DifferentChildren);
				foreach (DifferentChild dc in loadedParent.DifferentChildren)
				{
					dcId = dc.ID;
					break;
				}
			}

			using (ISession session = OpenSession())
			{
				DifferentChild dc = (DifferentChild) session.Get(typeof(DifferentChild), dcId);
			}

			using (ISession session = OpenSession())
			{
				session.Delete(dc1);
				session.Delete(dc2);
				session.Delete(child1);
				session.Delete(child2);
				session.Delete(parent);
				session.Delete(obj1);
				session.Delete(obj2);
				session.Flush();
			}
		}
	}
}