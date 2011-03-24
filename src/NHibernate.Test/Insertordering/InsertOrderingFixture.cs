using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using NHibernate.AdoNet;
using NHibernate.Cfg;
using NHibernate.Cfg.Loquacious;
using NHibernate.Connection;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.Insertordering
{
	public class InsertOrderingFixture : TestCase
	{
		const int batchSize = 10;
		const int instancesPerEach = 12;
		const int typesOfEntities = 3;
		protected override IList Mappings
		{
			get { return new[] {"Insertordering.Mapping.hbm.xml"}; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect.SupportsSqlBatches;
		}

		protected override void Configure(Configuration configuration)
		{
			configuration.DataBaseIntegration(x =>
			                                  {
																					x.BatchSize = batchSize;
																					x.OrderInserts = true;
																					x.Batcher<StatsBatcherFactory>();
			                                  });
		}

		[Test]
		public void BatchOrdering()
		{
			using (ISession s = OpenSession())
			using (s.BeginTransaction())
			{
				for (int i = 0; i < instancesPerEach; i++)
				{
					var user = new User {UserName = "user-" + i};
					var group = new Group {Name = "group-" + i};
					s.Save(user);
					s.Save(group);
					user.AddMembership(group);
				}
				StatsBatcher.Reset();
				s.Transaction.Commit();
			}

			int expectedBatchesPerEntity = (instancesPerEach / batchSize) + ((instancesPerEach % batchSize) == 0 ?  0 : 1);
			StatsBatcher.BatchSizes.Count.Should().Be(expectedBatchesPerEntity * typesOfEntities);

			using (ISession s = OpenSession())
			{
				s.BeginTransaction();
				IList users = s.CreateQuery("from User u left join fetch u.Memberships m left join fetch m.Group").List();
				foreach (object user in users)
				{
					s.Delete(user);
				}
				s.Transaction.Commit();
			}
		}

		#region Nested type: StatsBatcher

		public class StatsBatcher : SqlClientBatchingBatcher
		{
			private static string batchSQL;
			private static IList<int> batchSizes = new List<int>();
			private static int currentBatch = -1;

			public StatsBatcher(ConnectionManager connectionManager, IInterceptor interceptor)
				: base(connectionManager, interceptor) {}

			public static IList<int> BatchSizes
			{
				get { return batchSizes; }
			}

			public static void Reset()
			{
				batchSizes = new List<int>();
				currentBatch = -1;
				batchSQL = null;
			}

			public override IDbCommand PrepareBatchCommand(CommandType type, SqlString sql, SqlType[] parameterTypes)
			{
				IDbCommand result = base.PrepareBatchCommand(type, sql, parameterTypes);
				string sqlstring = sql.ToString();
				if (batchSQL == null || !sqlstring.Equals(batchSQL))
				{
					currentBatch++;
					batchSQL = sqlstring;
					batchSizes.Insert(currentBatch, 0);
					Console.WriteLine("--------------------------------------------------------");
					Console.WriteLine("Preparing statement [" + batchSQL + "]");
				}
				return result;
			}

			public override void AddToBatch(IExpectation expectation)
			{
				batchSizes[currentBatch]++;
				Console.WriteLine("Adding to batch [" + batchSQL + "]");
				base.AddToBatch(expectation);
			}

			protected override void DoExecuteBatch(IDbCommand ps)
			{
				Console.WriteLine("executing batch [" + batchSQL + "]");
				Console.WriteLine("--------------------------------------------------------");
				batchSQL = null;
				base.DoExecuteBatch(ps);
			}
		}

		#endregion

		#region Nested type: StatsBatcherFactory

		public class StatsBatcherFactory : IBatcherFactory
		{
			#region IBatcherFactory Members

			public IBatcher CreateBatcher(ConnectionManager connectionManager, IInterceptor interceptor)
			{
				return new StatsBatcher(connectionManager, interceptor);
			}

			#endregion
		}

		#endregion
	}
}