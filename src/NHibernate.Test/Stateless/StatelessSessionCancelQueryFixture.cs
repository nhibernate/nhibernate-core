using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.Stateless
{
	[TestFixture]
	public class StatelessSessionCancelQueryFixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override string[] Mappings
		{
			get { return new[] { "Stateless.Document.hbm.xml" }; }
		}

		private async Task CancelQueryTest(Action<IStatelessSession> queryAction)
		{
			if (!(Sfi.Dialect is PostgreSQLDialect))
			{
				Assert.Ignore();
			}

			using (IStatelessSession s = Sfi.OpenStatelessSession())
			using (var transactionS = s.BeginTransaction())
			{
				var command = s.Connection.CreateCommand();
				command.CommandText = @"
LOCK TABLE Document IN ACCESS EXCLUSIVE MODE;
";
				command.ExecuteNonQuery();

				using (IStatelessSession ss = Sfi.OpenStatelessSession())
				using (var transactionSS = ss.BeginTransaction())
				{
					var queryTask = Task.Factory.StartNew(() =>
					{
						queryAction.Invoke(ss);
					});

					Thread.Sleep(2000);
					ss.CancelQuery();
					Assert.CatchAsync(async () => { await queryTask; });
					try
					{
						await queryTask;
					}
					catch (Exception ex)
					{
						if (Sfi.Dialect is PostgreSQLDialect)
						{
							Assert.IsNotNull(ex.InnerException);
							Assert.AreEqual(ex.InnerException.GetType(), typeof(OperationCanceledException));
						}
					}
				}
			}
		}

		[Test]
		public async Task CancelHqlQuery()
		{
			await CancelQueryTest((s) => 
			{
				var documents = s.CreateQuery("from Document").List<Document>();
			});
		}

		[Test]
		public async Task CancelQuery()
		{
			await CancelQueryTest((s) =>
			{
				var documents = s.Query<Document>().ToList();
			});
		}

		[Test]
		public async Task CancelSQLQuery()
		{
			await CancelQueryTest((s) =>
			{
				var documents = s.CreateSQLQuery("select * from Document").List();
			});
		}
	}
}
