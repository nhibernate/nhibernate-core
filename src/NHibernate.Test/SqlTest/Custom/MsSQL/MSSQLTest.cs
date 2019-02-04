using System.Collections;
using System.Data;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.SqlTest.Custom.MsSQL
{
	[TestFixture]
	public class MSSQLTest : CustomStoredProcSupportTest
	{
		protected override string[] Mappings
		{
			get { return new[] { "SqlTest.Custom.MsSQL.MSSQLEmployment.hbm.xml" }; }
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is MsSql2000Dialect;
		}

		[Test]
		public void CanCallStoredProcedureWithTableParameterInferedAutomatically()
		{
			//NH-3736
			var createTypeSql = "CREATE TYPE dbo.TableType AS TABLE " +
			"( " +
			"	a INT, " +
			"	b INT " +
			")";
			var createProcedureSql = "CREATE PROCEDURE dbo.TableProcedure " +
			"( " +
			"	@t TableType READONLY " +
			") " +
			"AS " +
			"BEGIN " +
			"	SELECT * " +
			"	FROM @t " +
			"	RETURN @@ROWCOUNT " +
			"END";

			var deleteTypeSql = "DROP TYPE dbo.TableType";
			var deleteProcedureSql = "DROP PROCEDURE dbo.TableProcedure";

			using (var session = this.OpenSession())
			{
				session.CreateSQLQuery(createTypeSql).ExecuteUpdate();
				session.CreateSQLQuery(createProcedureSql).ExecuteUpdate();

				var table = new DataTable("dbo.TableType");
				table.Columns.Add("a", typeof (int));
				table.Columns.Add("b", typeof(int));
				table.Rows.Add(1, 2);

				var result = session.CreateSQLQuery("EXEC dbo.TableProcedure :t").SetParameter("t", table).List();

				session.CreateSQLQuery(deleteProcedureSql).ExecuteUpdate();
				session.CreateSQLQuery(deleteTypeSql).ExecuteUpdate();

				Assert.IsNotNull(result);
				Assert.IsNotEmpty(result);
			}
		}

		[Test]
		public void CanCallStoredProcedureWithTableParameterSetSpecifically()
		{
			//NH-3736
			var createTypeSql = "CREATE TYPE dbo.TableType AS TABLE " +
			"( " +
			"	a INT, " +
			"	b INT " +
			")";
			var createProcedureSql = "CREATE PROCEDURE dbo.TableProcedure " +
			"( " +
			"	@t TableType READONLY " +
			") " +
			"AS " +
			"BEGIN " +
			"	SELECT * " +
			"	FROM @t " +
			"	RETURN @@ROWCOUNT " +
			"END";

			var deleteTypeSql = "DROP TYPE dbo.TableType";
			var deleteProcedureSql = "DROP PROCEDURE dbo.TableProcedure";

			using (var session = this.OpenSession())
			{
				session.CreateSQLQuery(createTypeSql).ExecuteUpdate();
				session.CreateSQLQuery(createProcedureSql).ExecuteUpdate();

				var table = new DataTable("dbo.TableType");
				table.Columns.Add("a", typeof(int));
				table.Columns.Add("b", typeof(int));
				table.Rows.Add(1, 2);

				var result = session.CreateSQLQuery("EXEC dbo.TableProcedure :t").SetParameter("t", table, NHibernateUtil.Structured("dbo.TableType")).List();

				session.CreateSQLQuery(deleteProcedureSql).ExecuteUpdate();
				session.CreateSQLQuery(deleteTypeSql).ExecuteUpdate();

				Assert.IsNotNull(result);
				Assert.IsNotEmpty(result);
			}
		}
	}
}