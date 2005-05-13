using System;
using System.Collections;

using NUnit.Framework;

namespace NHibernate.Test.TransactionTest
{
	[TestFixture]
	public class TransactionFixture : TestCase
	{
		protected override IList Mappings
		{
			// Only need to be able to open a session, no tables required.
			get { return new string[0]; }
		}

		[Test]
		public void SecondTransactionShouldntBeCommitted()
		{
			using( ISession session = OpenSession() )
			{
				using( ITransaction t1 = session.BeginTransaction() )
				{
					t1.Commit();
				}

				using( ITransaction t2 = session.BeginTransaction() )
				{
					Assert.IsFalse( t2.WasCommitted );
					Assert.IsFalse( t2.WasRolledBack );
				}
			}
		}
	}
}
