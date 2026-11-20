using System;
using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1313
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using var session = OpenSession();
			using var transaction = session.BeginTransaction();

			var account = new Account { Id = 1, Name = "Account_1", RawNumber = 1, OldAccountNumber = 1001 };
			var order = new HoldClose
			{
				Account = account,
				CloseDate = new DateTime(2023, 1, 1)
			};

			session.Save(account);
			session.Save(order);
			transaction.Commit();
		}

		protected override void OnTearDown()
		{
			using var session = OpenSession();
			using var transaction = session.BeginTransaction();

			session.CreateQuery("delete from System.Object").ExecuteUpdate();
			transaction.Commit();
		}

		[Test]
		public void ManyToOneTargettingAFormula()
		{
			using var session = OpenSession();
			using var transaction = session.BeginTransaction();

			var result = session.Query<HoldClose>();

			Assert.That(result.ToList(), Has.Count.EqualTo(1));
			transaction.Commit();
		}

		[Test]
		public void ManyToOneTargettingAFormulaIsLoadedLazily()
		{
			using var session = OpenSession();
			using var transaction = session.BeginTransaction();

			var holdClose = session.Query<HoldClose>().Single();

			Assert.That(holdClose.Account.Name, Is.EqualTo("Account_1"));
			transaction.Commit();
		}
	}
}
