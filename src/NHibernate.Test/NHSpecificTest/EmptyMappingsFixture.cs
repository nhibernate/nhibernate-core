using System;
using System.Collections;
using System.Data;

using NHibernate.Transaction;

using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest
{
	/// <summary>
	/// This fixture contains no mappings, it is thus faster and can be used
	/// to run tests for basic features that don't require any mapping files
	/// to function.
	/// </summary>
	[TestFixture]
	public class EmptyMappingsFixture : TestCase
	{
		protected override IList Mappings
		{
			get { return new string[0]; }
		}

		[Test]
		public void BeginWithIsolationLevel()
		{
			using( ISession s = OpenSession() )
			using( ITransaction t = s.BeginTransaction( IsolationLevel.ReadCommitted ) )
			{
				AdoTransaction at = ( AdoTransaction ) t;
				Assert.AreEqual( IsolationLevel.ReadCommitted, at.IsolationLevel );
			}
		}

		[Test, ExpectedException( typeof( ObjectDisposedException ) )]
		public void ReconnectAfterClose()
		{
			using( ISession s = OpenSession() )
			{
				s.Close();
				s.Reconnect();
			}
		}

		[Test, ExpectedException( typeof( QueryException ) )]
		public void InvalidQuery()
		{
			using( ISession s = OpenSession() )
			{
				s.Find( "from SomeInvalidClass" );
			}
		}
	}
}
