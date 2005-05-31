using System;
using System.Collections;
using System.Data;

using NHibernate.Transaction;

using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest
{
	[TestFixture]
	public class TransactionFixture : TestCase
	{
		protected override IList Mappings
		{
			get { return new string[0]; }
		}

		[Test]
		public void BeginWithIsolationLevel()
		{
			using( ISession s = OpenSession() )
			using( ITransaction t = s.BeginTransaction( IsolationLevel.RepeatableRead ) )
			{
				AdoTransaction at = ( AdoTransaction ) t;
				Assert.AreEqual( IsolationLevel.RepeatableRead, at.IsolationLevel );
			}
		}
	}
}
