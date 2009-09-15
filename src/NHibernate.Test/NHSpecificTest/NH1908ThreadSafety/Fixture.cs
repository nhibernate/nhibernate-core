using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1908ThreadSafety
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		public void UsingFiltersIsThreadSafe()
		{
			var errors = new List<Exception>();
			var threads = new List<Thread>();
			for (int i = 0; i < 50; i++)
			{
				var thread = new Thread(() =>
				{
					try
					{
						ScenarioRunningWithMultiThreading();
					}
					catch (Exception ex)
					{
						lock (errors)
							errors.Add(ex);
					}
				});
				thread.Start();
				threads.Add(thread);
			}

			foreach (var thread in threads)
			{
				thread.Join();
			}

			Assert.AreEqual(0, errors.Count);
		}

		private void ScenarioRunningWithMultiThreading()
		{
			using (var session = sessions.OpenSession())
			{
				session
					.EnableFilter("CurrentOnly")
					.SetParameter("date", DateTime.Now);

				session.CreateQuery(
					@"
				select u
				from Order u
					left join fetch u.ActiveOrderLines
				where
					u.Email = :email
				")
					.SetString("email", "stupid@bugs.com")
					.UniqueResult<Order>();
			}
		}
	}
}
