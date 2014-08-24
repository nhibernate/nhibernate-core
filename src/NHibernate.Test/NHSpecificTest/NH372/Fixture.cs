using System;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH372
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected bool isDynamic;

		public override string BugNumber
		{
			get { return "NH372"; }
		}

		private void ComponentFieldNotInserted_Generic(System.Type type)
		{
			int id;

			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				BaseParent p = (BaseParent) Activator.CreateInstance(type);
				p.Component.FieldNotInserted = 10;
				session.Save(p);

				tx.Commit();

				id = p.Id;
			}

			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				BaseParent p = (BaseParent) session.Get(type, id);

				Assert.AreEqual(0, p.Component.FieldNotInserted,
				                "Field should not have been inserted.");

				tx.Commit();
			}
		}

		[Test]
		public void ComponentFieldNotInserted()
		{
			isDynamic = false;
			ComponentFieldNotInserted_Generic(typeof(Parent));
		}

		[Test]
		public void ComponentFieldNotInserted_Dynamic()
		{
			isDynamic = true;
			ComponentFieldNotInserted_Generic(typeof(DynamicParent));
		}

		private void ComponentFieldNotUpdated_Generic(System.Type type)
		{
			int id;
			int fieldInitialValue = 10;
			int fieldNewValue = 30;

			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				BaseParent p = (BaseParent) Activator.CreateInstance(type);
				p.Component.FieldNotUpdated = fieldInitialValue;
				session.Save(p);

				tx.Commit();

				id = p.Id;
			}

			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				BaseParent p = (BaseParent) session.Get(type, id);

				Assert.AreEqual(fieldInitialValue, p.Component.FieldNotUpdated,
				                String.Format("Field should have initial inserted value of {0}.", fieldInitialValue));

				p.Component.FieldNotUpdated = fieldNewValue;
				p.Component.NormalField = 10;

				tx.Commit();
			}

			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				BaseParent p = (BaseParent) session.Get(type, id);

				Assert.AreEqual(fieldInitialValue, p.Component.FieldNotUpdated,
				                "Field should not have been updated.");

				tx.Commit();
			}
		}

		[Test]
		public void ComponentFieldNotUpdated()
		{
			isDynamic = false;
			ComponentFieldNotUpdated_Generic(typeof(Parent));
		}

		[Test]
		public void ComponentFieldNotUpdated_Dynamic()
		{
			isDynamic = true;
			ComponentFieldNotUpdated_Generic(typeof(DynamicParent));
		}

		private void SubComponentFieldNotInserted_Generic(System.Type type)
		{
			int id;

			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				BaseParent p = (BaseParent) Activator.CreateInstance(type);
				p.Component.SubComponent.FieldNotInserted = 10;
				session.Save(p);

				tx.Commit();

				id = p.Id;
			}

			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				BaseParent p = (BaseParent) session.Get(type, id);

				Assert.AreEqual(0, p.Component.SubComponent.FieldNotInserted,
				                "Field should not have been inserted.");

				tx.Commit();
			}
		}

		[Test]
		public void SubComponentFieldNotInserted()
		{
			isDynamic = false;
			SubComponentFieldNotInserted_Generic(typeof(Parent));
		}

		[Test]
		public void SubComponentFieldNotInserted_Dynamic()
		{
			isDynamic = false;
			SubComponentFieldNotInserted_Generic(typeof(DynamicParent));
		}

		private void SubComponentFieldNotUpdated_Generic(System.Type type)
		{
			int id;
			int fieldInitialValue = 10;
			int fieldNewValue = 30;

			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				BaseParent p = (BaseParent) Activator.CreateInstance(type);
				p.Component.SubComponent.FieldNotUpdated = fieldInitialValue;
				session.Save(p);

				tx.Commit();

				id = p.Id;
			}

			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				BaseParent p = (BaseParent) session.Get(type, id);

				Assert.AreEqual(fieldInitialValue, p.Component.SubComponent.FieldNotUpdated,
				                String.Format("Field should have initial inserted value of {0}.", fieldInitialValue));

				p.Component.SubComponent.FieldNotUpdated = fieldNewValue;
				p.Component.SubComponent.NormalField = 10;

				tx.Commit();
			}

			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				BaseParent p = (BaseParent) session.Get(type, id);

				Assert.AreEqual(fieldInitialValue, p.Component.SubComponent.FieldNotUpdated,
				                "Field should not have been updated.");

				tx.Commit();
			}
		}

		[Test]
		public void SubComponentFieldNotUpdated()
		{
			isDynamic = false;
			SubComponentFieldNotUpdated_Generic(typeof(Parent));
		}

		[Test]
		public void SubComponentFieldNotUpdated_Dynamic()
		{
			isDynamic = false;
			SubComponentFieldNotUpdated_Generic(typeof(DynamicParent));
		}

		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				if (isDynamic)
				{
					session.Delete("from DynamicParent");
				}
				else
				{
					session.Delete("from Parent");
				}
				tx.Commit();
			}
		}
	}
}