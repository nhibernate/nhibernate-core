using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH962
{
	[TestFixture]
	public class NH962Fixture : BugTestCase
	{
		[Test]
		public void Bug()
		{
			Parent parent = new Parent();
			parent.Name = "Test Parent";

			Child child = new Child();
			child.Name = "Test Child";

			child.Parent = parent;
			parent.Children = new HashSet<Child>();
			parent.Children.Add(child);

			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				session.Save(child);
				Assert.IsTrue(session.Contains(parent));
				Assert.AreNotEqual(Guid.Empty, parent.Id);
				tx.Commit();
			}

			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				session.Delete(child);
				tx.Commit();
			}
		}
	}
}
