using System;
using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH335
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH335"; }
		}

		private AbcThing[] abcThings;
		private OtherThing[] otherThings;

		private int numAbcThings;
		private int numOtherThings;

		protected override void OnSetUp()
		{
			base.OnSetUp();

			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				// Insert a bunch of AbcThings
				numAbcThings = 5;
				this.abcThings = new AbcThing[numAbcThings];
				for (int i = 0; i < numAbcThings; i++)
				{
					AbcThing newAbcThing = new AbcThing();
					// AbcThing.ClassType is automatically generated.
					newAbcThing.ID = Utils.GetRandomID();
					newAbcThing.Name = newAbcThing.ID;
					session.Save(newAbcThing);
				}

				// Insert a bunch of OtherThings
				numOtherThings = 5;
				this.otherThings = new OtherThing[numOtherThings];
				for (int i = 0; i < numOtherThings; i++)
				{
					OtherThing newOtherThing = new OtherThing();
					// OtherThing.ClassType is automatically generated.
					newOtherThing.ID = Utils.GetRandomID();
					newOtherThing.Name = newOtherThing.ID;
					session.Save(newOtherThing);
				}

				tx.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				session.Delete("from AbcThing");
				session.Delete("from OtherThing");
				tx.Commit();
			}

			base.OnTearDown();
		}

		[Test]
		public void SelectSuperclass()
		{
			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				IQuery query = session.CreateQuery("from Thing");
				IList list = query.List();

				Assert.AreEqual(numAbcThings + numOtherThings, list.Count,
				                String.Format("There should be {0} Things.", numAbcThings + numOtherThings));

				foreach (object thing in list)
				{
					Assert.IsTrue(thing is Thing);
				}
				tx.Commit();
			}
		}

		[Test]
		public void SelectSubclass()
		{
			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				IQuery query = session.CreateQuery("from AbcThing");
				IList list = query.List();

				Assert.AreEqual(numAbcThings, list.Count,
				                String.Format("There should be {0} AbcThings.", numAbcThings));

				foreach (object thing in list)
				{
					Assert.IsTrue(thing is AbcThing);
				}
				tx.Commit();
			}

			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				IQuery query = session.CreateQuery("from OtherThing");
				IList list = query.List();

				Assert.AreEqual(numAbcThings, list.Count,
				                String.Format("There should be {0} OtherThings.", numAbcThings));

				foreach (object thing in list)
				{
					Assert.IsTrue(thing is OtherThing);
				}

				tx.Commit();
			}
		}

		[Test]
		public void Delete()
		{
			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				session.Delete("from AbcThing");
				tx.Commit();
			}

			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				IQuery abcThingQuery = session.CreateQuery("from AbcThing");
				IList abcThings = abcThingQuery.List();

				Assert.AreEqual(0, abcThings.Count,
				                "All AbcThings should have been deleted.");

				IQuery otherThingQuery = session.CreateQuery("from OtherThing");
				IList otherThings = otherThingQuery.List();

				Assert.AreEqual(numOtherThings, otherThings.Count,
				                "No OtherThings should have been deleted.");

				tx.Commit();
			}
		}
	}
}