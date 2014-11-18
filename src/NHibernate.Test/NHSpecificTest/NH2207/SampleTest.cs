using System;
using System.Data;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2207
{
	[TestFixture, Ignore("Demostration of external issue")]
	public class SampleTest : BugTestCase
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect as MsSql2008Dialect != null;
		}

		[Test]
		public void WithoutUseNHSqlDataProviderWorkProperly()
		{
			var createTable = "CREATE TABLE TryDate([Id] [int] IDENTITY(1,1) NOT NULL,[MyDate] [date] NOT NULL)";
			var dropTable = "DROP TABLE TryDate";
			var insertTable = "INSERT INTO TryDate([MyDate]) VALUES(@p0)";
			using(var sqlConnection = new System.Data.SqlClient.SqlConnection(cfg.Properties[Cfg.Environment.ConnectionString]))
			{
				sqlConnection.Open();
				using (var tx = sqlConnection.BeginTransaction())
				{
					var command = sqlConnection.CreateCommand();
					command.Transaction = tx;
					command.CommandText = createTable;
					command.ExecuteNonQuery();
					tx.Commit();
				}

				try
				{
					using (var tx = sqlConnection.BeginTransaction())
					{
						var command = sqlConnection.CreateCommand();
						command.Transaction = tx;
						command.CommandText = insertTable;
						var dateParam = command.CreateParameter();
						dateParam.ParameterName = "@p0";
						dateParam.DbType = DbType.Date;
						dateParam.Value = DateTime.MinValue.Date;
						command.Parameters.Add(dateParam);
						command.ExecuteNonQuery();
						tx.Commit();
					}
				}
				finally
				{
					using (var tx = sqlConnection.BeginTransaction())
					{
						var command = sqlConnection.CreateCommand();
						command.Transaction = tx;
						command.CommandText = dropTable;
						command.ExecuteNonQuery();
						tx.Commit();
					}					
				}
			}

		}

		[Test]
		public void Dates_Before_1753_Should_Not_Insert_Null()
		{
			object savedId;
			var expectedStoredValue = DateTime.MinValue.Date.AddDays(1).Date;
			using (ISession session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				var concrete = new DomainClass{Date = expectedStoredValue.AddMinutes(90)};
				savedId = session.Save(concrete);
				tx.Commit();
			}

			using (ISession session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				var savedObj = session.Get<DomainClass>(savedId);
				Assert.That(savedObj.Date, Is.EqualTo(expectedStoredValue));
				session.Delete(savedObj);
				tx.Commit();
			}
		}
	}
}