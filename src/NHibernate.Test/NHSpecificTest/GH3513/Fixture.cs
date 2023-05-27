using System;
using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH3513
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var account = new Account { Id = 1, Name = "Account_1", OldAccountNumber = 1 };

				var order = new HoldClose
				{
					Account = account,
					CloseDate = new DateTime(2023, 1, 1)
				};

				session.Save(account);
				session.Save(order);
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
				// instead if in need of having NHibernate ordering the deletes, but this will cause
				// loading the entities in the session.
				session.CreateQuery("delete from System.Object").ExecuteUpdate();

				transaction.Commit();
			}
		}

		[Test]
		public void ManyToOneTargettingAFormula()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = from e in session.Query<HoldClose>()
							 select e;

				Assert.That(result.ToList(), Has.Count.EqualTo(1));
			}
		}
	}
}
