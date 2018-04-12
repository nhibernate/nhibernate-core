using System;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1645
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		private Guid _superParentId;
		private Guid _parentId;

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var p = new Parent();
				session.Save(p);
				_parentId = p.Id;

				_superParentId = (Guid) session.Save(new SuperParent { Parent = p });

				transaction.Commit();
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
				session.CreateQuery("delete from System.Object").ExecuteUpdate();

				transaction.Commit();
			}
		}

		[Test]
		public void SOEOnLoad()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var superParent = session.Load<SuperParent>(_superParentId);
				Assert.That(() => NHibernateUtil.Initialize(superParent), Throws.Nothing);
				Assert.That(() => NHibernateUtil.Initialize(superParent.Parent), Throws.Nothing);
			}
		}
	}
}
