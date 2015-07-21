using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1908ThreadSafety
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return !(dialect is Dialect.Oracle8iDialect);
			// Oracle sometimes causes: ORA-12520: TNS:listener could not find available handler for requested type of server
			// Following links bizarrely suggest it's an Oracle limitation under load:
			// http://www.orafaq.com/forum/t/60019/2/ & http://www.ispirer.com/wiki/sqlways/troubleshooting-guide/oracle/import/tns_listener
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();

			if (!(Dialect is Dialect.FirebirdDialect))
				return;

			// Firebird will pool each connection created during the test and will not drop the created tables
			// which will result in other tests failing when they try to create tables with same name
			// By clearing the connection pool the tables will get dropped. This is done by the following code.
			var fbConnectionType = ReflectHelper.TypeFromAssembly("FirebirdSql.Data.FirebirdClient.FbConnection", "FirebirdSql.Data.FirebirdClient", false);
			var clearPool = fbConnectionType.GetMethod("ClearPool");
			var sillyConnection = sessions.ConnectionProvider.GetConnection();
			clearPool.Invoke(null, new object[] { sillyConnection });
			sessions.ConnectionProvider.CloseConnection(sillyConnection);
		}

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

			Console.WriteLine("Detected {0} errors in threads. Displaying the first three (if any):", errors.Count);
			foreach (var exception in errors.Take(3))
				Console.WriteLine(exception);

			Assert.AreEqual(0, errors.Count, "number of threads with exceptions");
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
