using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2919
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		private Guid _loadedParentID;
		private Guid _loadedSummaryID;

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				Child.CREATE_WITH_TOY = false;
				var parent = new Parent {Name = "Parent"};
				var child = new Child() {Parent = parent};
				parent.Children = new HashSet<Child>();
				parent.Children.Add(child);
				parent.Summary = new ParentSummary() {Parent = parent};

				session.Save(parent);
				session.Flush();
				transaction.Commit();

				_loadedParentID = parent.ID;
				_loadedSummaryID = parent.Summary.ID;
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				// The HQL delete does all the job inside the database without loading the entities, but it does
				// not handle delete order for avoiding violating constraints if any. Use
				// session.Delete("from System.Object");
				// instead if in need of having NHbernate ordering the deletes, but this will cause
				// loading the entities in the session.
				session.Delete("from System.Object");

				transaction.Commit();
			}
		}

		[Test]
		public void EntityModifiedDuringFlush()
		{
			Child.CREATE_WITH_TOY = true;

			// TEST 1:
			// Will only load the parent - will only persist parent modifications
			using (var session = OpenSession())
			{
				var parent = session.Get<Parent>(_loadedParentID);

				session.Flush();
			}

			// TEST 2:
			// Will only load the parent.  (Just getting the identifier will not load the entity)
			// Will only persist parent modifications.
			using (var session = OpenSession())
			{
				var parent = session.Get<Parent>(_loadedParentID);
				parent.Name = "Changing Name";
				var id = parent.Summary.ID;

				session.Flush();
			}

			// TEST 3:
			// Will load the parent and the summary.  As a result, upon persistence, it will load 
			// the Children because of a property in summary that will force the Children to load.
			// However, it will not cascade through the Child object's properties.
			using (var session = OpenSession())
			{
				var parent = session.Get<Parent>(_loadedParentID);
				parent.Name = "New";
				parent.Summary.Parent = parent;

				session.Flush();
			}

			// TEST 4:
			// Will also load the parent and the summary.  As a result, upon persistence, it will load
			// the Children because of a property in summary that will force the Children to load.
			// However, it will not cascade through Child object's properties.
			using (var session = OpenSession())
			{
				var summary = session.Get<ParentSummary>(_loadedSummaryID);
				var parent = session.Get<Parent>(_loadedParentID);

				summary.Parent = parent;

				session.Flush();
			}
		}
	}
}
