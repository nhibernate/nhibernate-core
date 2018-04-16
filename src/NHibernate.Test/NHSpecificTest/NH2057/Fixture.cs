using System.Threading;
using System.Transactions;
using NHibernate.Engine;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2057
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override bool AppliesTo(ISessionFactoryImplementor factory) =>
			factory.ConnectionProvider.Driver.SupportsSystemTransactions;

		[Test]
		public void WillCloseWhenUsingDTC()
		{
			ISession s;
			using (var tx = new TransactionScope())
			{
				using (s = OpenSession())
				{
					s.Get<Person>(1);
				}
				//not closed because the tx is opened yet
				Assert.That(s.IsOpen, Is.True);
				tx.Complete();
			}
			// ODBC does promote to distributed, causing the completion to happen concurrently to code
			// following scope disposal. Sleep a bit for accounting for this.
			Assert.That(() => s.IsOpen, Is.False.After(500, 100));
		}
	}
}
