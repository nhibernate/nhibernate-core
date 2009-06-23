using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1850
{
	using AdoNet;

	[TestFixture]
	public class Fixture:BugTestCase
	{
		[Test]
		public void CanGetQueryDurationForDelete()
		{
			using (LogSpy spy = new LogSpy(typeof(AbstractBatcher)))
			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				session.CreateQuery("delete Customer").ExecuteUpdate();

				var wholeLog = spy.GetWholeLog();
				Assert.True(
					wholeLog.Contains("ExecuteNonQuery took")
					);

				tx.Rollback();
			}
		}

		[Test]
		public void CanGetQueryDurationForSelect()
		{
			using (LogSpy spy = new LogSpy(typeof(AbstractBatcher)))
			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				session.CreateQuery("from Customer").List();

				var wholeLog = spy.GetWholeLog();
				Assert.True(
					wholeLog.Contains("ExecuteReader took")
					);
				Assert.True(
					wholeLog.Contains("DataReader was closed after")
					);

				tx.Rollback();
			}
		}
	}
}
