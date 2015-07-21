using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1747
{
	[TestFixture]
	public class JoinTraversalTest : BugTestCase
	{
		[Test]
		public void PaymentTableNotContainsFk()
		{
			var cm = cfg.GetClassMapping(typeof (Payment));
			bool containsFK= false;
			foreach (var column in cm.Table.ColumnIterator)
			{
				if (column.Name.Equals("PaymentBatchFK_ID"))
					containsFK = true;
			}
			Assert.That(containsFK, Is.False);
		}

		protected override void OnSetUp()
		{
			base.OnSetUp();
			using (ISession session = OpenSession())
			{
				var payment = new Payment { Amount = 5m, Id = 1 };
				var paymentBatch = new PaymentBatch { Id = 3 };
				paymentBatch.AddPayment(payment);
				session.Save(paymentBatch);
				session.Save(payment);
				session.Flush();
			}
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();
			using (ISession session = OpenSession())
			{
				var hql = "from System.Object";
				session.Delete(hql);
				session.Flush();
			}
		}

		[Test]
		public void TraversingBagToJoinChildElementShouldWork()
		{
			using (ISession session = OpenSession())
			{
				var paymentBatch = session.Get<PaymentBatch>(3);
				Assert.AreEqual(1, paymentBatch.Payments.Count);
			}
		}
	}

}
