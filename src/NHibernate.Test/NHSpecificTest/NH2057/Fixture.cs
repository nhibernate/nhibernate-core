using System;
using System.Collections.Generic;
using System.Data;
using System.Transactions;
using NHibernate.Impl;
using NUnit.Framework;
using NHibernate.Criterion;

namespace NHibernate.Test.NHSpecificTest.NH2057
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		[Description("This test fails intermittently on SQL Server ODBC. Not sure why.")]
		public void WillCloseWhenUsingDTC()
		{
			SessionImpl s;
			using (var tx = new TransactionScope())
			{
				using (s = (SessionImpl)OpenSession())
				{
					s.Get<Person>(1);
				}
				//not closed because the tx is opened yet
				Assert.False(s.IsClosed);
				tx.Complete();
			}
			Assert.That(s.IsClosed, Is.True);
		}
	}
}
