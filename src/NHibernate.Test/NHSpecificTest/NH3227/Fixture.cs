using System;
using System.Collections.Generic;
using System.Threading;
using NHibernate.AdoNet;
using NHibernate.Impl;
using NUnit.Framework;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Test.NHSpecificTest.NH3227
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH3227"; }
		}

		protected override void Configure(NHibernate.Cfg.Configuration configuration)
		{
			configuration.SetProperty(Environment.BatchSize, "1");
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect.SupportsSqlBatches;
		}
		
		[Test]
		public void CanCloseCommandsWhenCommandsAreBeingRun()
		{
			//used to get exception from other threads
			Exception threadException = null;

			using (LogSpy spy = new LogSpy(typeof(AbstractBatcher)))
			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				var threads = new List<Thread>();
				for (int i = 0; i < 10; i++)
				{
					threads.Add(new Thread(s =>
					{
						try
						{
							session.CreateSQLQuery("WAITFOR DELAY '00:00:01'").ExecuteUpdate();
						}
						catch (Exception ex)
						{
							threadException = ex;
						}
					}));
				}

				foreach (var t in threads)
				{
					t.Start();
				}

				var closeCommandThread = new Thread(s =>
												{
													try
													{
														while (true)
														{
															((SessionImpl)session).Batcher.CloseCommands();
														}
													}
													catch (ThreadAbortException)
													{
													}
													catch (Exception ex)
													{
														threadException = ex;
													}
												});
				closeCommandThread.Start();

				foreach (var t in threads)
				{
					t.Join();
				}

				closeCommandThread.Abort();
				
				tx.Rollback();
			}

			if (threadException != null)
			{
				throw threadException;
			}
		}
	}
}
