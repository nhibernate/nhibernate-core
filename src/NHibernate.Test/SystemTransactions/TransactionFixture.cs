using System.Collections;
using System.Transactions;
using NHibernate.DomainModel;
using NUnit.Framework;

namespace NHibernate.Test.SystemTransactions
{
	[TestFixture]
	public class TransactionFixture : TestCase
	{
		protected override IList Mappings
		{
			get { return new string[] { "WZ.hbm.xml" }; }
		}

		[Test]
		public void CanUseSystemTransactionsToCommit()
		{
			object identifier;
			using(ISession session = Sfi.OpenSession())
			using(TransactionScope tx = new TransactionScope())
			{
				W s = new W();
				session.Save(s);
				identifier = s.Id;
				tx.Complete();
			}

			using (ISession session = Sfi.OpenSession())
			using (TransactionScope tx = new TransactionScope())
			{
				W w = session.Get<W>(identifier);
				Assert.IsNotNull(w);
				session.Delete(w);
				tx.Complete();
			}
		}
	}
}