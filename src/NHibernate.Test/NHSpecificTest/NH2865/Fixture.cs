using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;
using System;

namespace NHibernate.Test.NHSpecificTest.NH2865
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var e1 = new OrderLine {Quantity = "2"};
				session.Save(e1);

				var e2 = new OrderLine { Quantity = "3" };
				session.Save(e2);

				session.Flush();
				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");

				session.Flush();
				transaction.Commit();
			}
		}

		[Test]
		public void UsingConvertToInt32InSumExpressionShouldNotThrowException()
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.Query<OrderLine>().Sum(l => int.Parse(l.Quantity));

				Assert.AreEqual(5, result);
			}
		}
	}
}