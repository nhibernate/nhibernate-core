using System;
using System.Collections;
using System.Data;
using NHibernate.Exceptions;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.ExceptionsTest
{
	[TestFixture]
	public class SQLExceptionConversionTest : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new string[] { "ExceptionsTest.User.hbm.xml", "ExceptionsTest.Group.hbm.xml" }; }
		}


		[Test, Ignore("Not supported yet.")]
		public void IntegrityViolation()
		{
			if (Dialect is Dialect.MySQLDialect)
			{
				Assert.Ignore("MySQL (ISAM) does not support FK violation checking", "exception conversion");
				return;
			}

			ISQLExceptionConverter converter = Dialect.BuildSQLExceptionConverter();

			ISession session = OpenSession();
			session.BeginTransaction();
			IDbConnection connection = session.Connection;

			// Attempt to insert some bad values into the T_MEMBERSHIP table that should
			// result in a constraint violation
			IDbCommand ps = null;
			try
			{
				ps = connection.CreateCommand();
				ps.CommandType = CommandType.Text;
				ps.CommandText = "INSERT INTO T_MEMBERSHIP (user_id, group_id) VALUES (@p1, @p2)";
				IDbDataParameter pr = ps.CreateParameter();
				pr.ParameterName = "p1";
				pr.DbType = DbType.Int64;
				pr.Value = 52134241L; // Non-existent user_id
				ps.Parameters.Add(pr);

				pr = ps.CreateParameter();
				pr.ParameterName = "p2";
				pr.DbType = DbType.Int64;
				pr.Value = 5342L; // Non-existent group_id
				ps.Parameters.Add(pr);

				session.Transaction.Enlist(ps);
				ps.ExecuteNonQuery();

				Assert.Fail("INSERT should have failed");
			}
			catch (Exception sqle)
			{
				ADOExceptionReporter.LogExceptions(sqle, "Just output!!!!");
				ADOException adoException = converter.Convert(sqle, null, null);
				Assert.AreEqual(typeof(ConstraintViolationException), adoException.GetType(),
												"Bad conversion [" + sqle.Message + "]");
				ConstraintViolationException ex = (ConstraintViolationException)adoException;
				Console.WriteLine("Violated constraint name: " + ex.ConstraintName);
			}
			finally
			{
				if (ps != null)
				{
					try
					{
						ps.Dispose();
					}
					catch (Exception)
					{
						// ignore...
					}
				}
			}

			session.Transaction.Rollback();
			session.Close();
		}

		[Test, Ignore("Not supported yet.")]
		public void BadGrammar()
		{
			ISQLExceptionConverter converter = Dialect.BuildSQLExceptionConverter();

			ISession session = OpenSession();
			IDbConnection connection = session.Connection;

			// prepare/execute a query against a non-existent table
			IDbCommand ps = null;
			try
			{
				ps = connection.CreateCommand();
				ps.CommandType = CommandType.Text;
				ps.CommandText = "SELECT user_id, user_name FROM tbl_no_there";
				ps.ExecuteNonQuery();

				Assert.Fail("SQL compilation should have failed");
			}
			catch (Exception sqle)
			{
				Assert.AreEqual(typeof(SQLGrammarException), converter.Convert(sqle, null, null).GetType(), "Bad conversion [" + sqle.Message + "]");
			}
			finally
			{
				if (ps != null)
				{
					try
					{
						ps.Dispose();
					}
					catch (Exception)
					{
						// ignore...
					}
				}
			}

			session.Close();
		}
	}
}
