using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Transactions;
using log4net;
using log4net.Repository.Hierarchy;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Engine;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3023
{
	[TestFixture]
	public class DeadlockConnectionPoolIssueTest : BugTestCase
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof(DeadlockConnectionPoolIssueTest));

		// Uses directly SqlConnection.
		protected override bool AppliesTo(ISessionFactoryImplementor factory)
			=> factory.ConnectionProvider.Driver is SqlClientDriver && base.AppliesTo(factory);

		protected override bool AppliesTo(Dialect.Dialect dialect)
			=> dialect is MsSql2000Dialect && base.AppliesTo(dialect);

		protected override void OnSetUp()
		{
			RunScript("db-seed.sql");

			((Logger)_log.Logger).Level = log4net.Core.Level.Debug;
		}

		protected override void OnTearDown()
		{
			//
			// Hopefully this will clean up the pool so that teardown can succeed
			//
			SqlConnection.ClearAllPools();

			using (var s = OpenSession())
			{
				s.CreateQuery("delete from System.Object").ExecuteUpdate();
			}
			RunScript("db-teardown.sql");
		}

		[Theory]
		public void ConnectionPoolCorruptionAfterDeadlock(bool distributed)
		{
			var tryCount = 0;
			var id = 1;
			var missingDeadlock = false;
			do
			{
				tryCount++;

				try
				{
					_log.DebugFormat("Starting loop {0}", tryCount);
					// When the connection is released from transaction completion, the scope disposal after deadlock
					// takes up to 30 seconds (not at first try, but at subsequent tries). With additional logs, it
					// appears this delay occurs at connection closing. Definitely, there is something which can go
					// wrong when disposing a connection from transaction scope completion.
					// Note that the transaction completion event can execute as soon as the deadlock occurs. It does
					// not wait for the scope disposal.
					using (var session = OpenSession())
					//using (var session = Sfi.WithOptions().ConnectionReleaseMode(ConnectionReleaseMode.OnClose).OpenSession())
					using (var scope = distributed ? CreateDistributedTransactionScope() : new TransactionScope())
					{
						_log.Debug("Session and scope opened");
						session.GetSessionImplementation().Factory.TransactionFactory
							.EnlistInSystemTransactionIfNeeded(session.GetSessionImplementation());
						_log.Debug("Session enlisted");
						try
						{
							new DeadlockHelper().ForceDeadlockOnConnection((SqlConnection)session.Connection);
						}
						catch (SqlException x)
						{
							//
							// Deadlock error code is 1205.
							//
							if (x.Errors.Cast<SqlError>().Any(e => e.Number == 1205))
							{
								//
								// It did what it was supposed to do.
								//
								_log.InfoFormat("Expected deadlock on attempt {0}. {1}", tryCount, x.Message);

								// Check who takes time in the disposing
								var chrono = new Stopwatch();
								chrono.Start();
								scope.Dispose();
								_log.Debug("Scope disposed");
								Assert.That(chrono.Elapsed, Is.LessThan(TimeSpan.FromSeconds(2)), "Abnormal scope disposal duration");
								chrono.Restart();
								session.Dispose();
								_log.Debug("Session disposed");
								Assert.That(chrono.Elapsed, Is.LessThan(TimeSpan.FromSeconds(2)), "Abnormal session disposal duration");
								continue;
							}

							//
							// ? This shouldn't happen
							//
							Assert.Fail("Surprising exception when trying to force a deadlock: {0}", x);
						}

						_log.WarnFormat("Initial session seemingly not deadlocked at attempt {0}", tryCount);
						missingDeadlock = true;

						try
						{
							session.Save(
								new DomainClass
								{
									Id = id++,
									ByteData = new byte[] { 1, 2, 3 }
								});

							session.Flush();
							if (tryCount < 10)
							{
								_log.InfoFormat("Initial session still usable, trying again");
								continue;
							}
							_log.InfoFormat("Initial session still usable after {0} attempts, finishing test", tryCount);
						}
						catch (Exception ex)
						{
							_log.Error("Failed to continue using the session after lacking deadlock.", ex);
							// This exception would hide the transaction failure, if any.
							//throw;
						}
						_log.Debug("Completing scope");
						scope.Complete();
						_log.Debug("Scope completed");
					}
					_log.Debug("Session and scope disposed");
				}
				catch (AssertionException)
				{
					throw;
				}
				catch (Exception x)
				{
					_log.Error($"Initial session failed at attempt {tryCount}.", x);
				}

				var subsequentFailedRequests = 0;

				for (var i = 1; i <= 10; i++)
				{
					//
					// The error message will vary on subsequent requests, so we'll somewhat
					// arbitrarily try 10
					//

					try
					{
						using (var scope = new TransactionScope())
						{
							using (var session = OpenSession())
							{
								session.Save(
									new DomainClass
									{
										Id = id++,
										ByteData = new byte[] { 1, 2, 3 }
									});

								session.Flush();
							}

							scope.Complete();
						}
					}
					catch (Exception x)
					{
						subsequentFailedRequests++;
						_log.Error($"Subsequent session {i} failed.", x);
					}
				}

				Assert.Fail("{0}; {1} subsequent requests failed.",
							missingDeadlock
								? "Deadlock not reported on initial request, and initial request failed"
								: "Initial request failed",
							subsequentFailedRequests);

			} while (tryCount < 3);
			//
			// I'll change this to while(true) sometimes so I don't have to keep running the test
			//
		}

		private static TransactionScope CreateDistributedTransactionScope()
		{
			var scope = new TransactionScope();
			//
			// Forces promotion to distributed transaction
			//
			TransactionInterop.GetTransmitterPropagationToken(System.Transactions.Transaction.Current);
			return scope;
		}

		private void RunScript(string script)
		{
			var cxnString = cfg.Properties["connection.connection_string"] + "; Pooling=No";
			// Disable connection pooling so this won't be hindered by
			// problems encountered during the actual test

			string sql;
			using (var reader = new StreamReader(GetType().Assembly.GetManifestResourceStream(GetType().Namespace + "." + script)))
			{
				sql = reader.ReadToEnd();
			}

			using (var cxn = new SqlConnection(cxnString))
			{
				cxn.Open();

				foreach (var batch in Regex.Split(sql, @"^go\s*$", RegexOptions.IgnoreCase | RegexOptions.Multiline)
					.Where(b => !string.IsNullOrEmpty(b)))
				{

					using (var cmd = new System.Data.SqlClient.SqlCommand(batch, cxn))
					{
						cmd.ExecuteNonQuery();
					}
				}
			}
		}
	}
}
