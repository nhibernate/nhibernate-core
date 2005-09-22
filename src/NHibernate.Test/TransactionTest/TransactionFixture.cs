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
			// The mapping is only actually needed in one test
			get { return new string[ ] { "Simple.hbm.xml" }; }
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

		[Test]
		public void CommandAfterTransactionShouldWork()
		{
			using( ISession s = OpenSession() )
			{
				using( ITransaction t = s.BeginTransaction() )
				{
				}

				s.Find( "from Simple" );

				using( ITransaction t = s.BeginTransaction() )
				{
					t.Commit();
				}

				s.Find( "from Simple" );

				using( ITransaction t = s.BeginTransaction() )
				{
					t.Rollback();
				}

				s.Find( "from Simple" );
			}
		}
	}
}
