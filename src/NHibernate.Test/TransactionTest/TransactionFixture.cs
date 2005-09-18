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

		[Test]
		[ExpectedException( typeof( ObjectDisposedException ) )]
		public void CommitAfterDisposeThrowsException()
		{
			using( ISession s = OpenSession() )
			{
				ITransaction t = s.BeginTransaction();
				t.Dispose();
				t.Commit();
			}
		}

		[Test]
		[ExpectedException( typeof( ObjectDisposedException ) )]
		public void RollbackAfterDisposeThrowsException()
		{
			using( ISession s = OpenSession() )
			{
				ITransaction t = s.BeginTransaction();
				t.Dispose();
				t.Rollback();
			}
		}

		[Test]
		[ExpectedException( typeof( ObjectDisposedException ) )]
		public void EnlistAfterDisposeThrowsException()
		{
			using( ISession s = OpenSession() )
			{
				ITransaction t = s.BeginTransaction();
				t.Dispose();
				t.Enlist( null );
			}
		}
	}
}
