using System.Data;
using FirebirdSql.Data.FirebirdClient;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.SqlCommand;
using NUnit.Framework;

namespace NHibernate.Test.DialectTest
{
	[TestFixture]
	public class FirebirdDialectFixture
	{
		readonly FirebirdDialect _dialect = new FirebirdDialect();

		[Test]
		public void CreateSelectDrop()
		{
			var cfg = TestConfigurationHelper.GetDefaultConfiguration();
			var cnxStr = cfg.Properties[Environment.ConnectionString];
			using (var cnx = new FbConnection(cnxStr))
			{
				cnx.Open();
				using (var tran = cnx.BeginTransaction())
				{
					using (var cmd = cnx.CreateCommand())
					{
						cmd.Transaction = tran;
						cmd.CommandType = CommandType.Text;
						cmd.CommandText = "create table test (id int not null primary key)";
						cmd.ExecuteNonQuery();
					}
					tran.Commit();
				}
			}

			using (var cnx = new FbConnection(cnxStr))
			{
				cnx.Open();
				using (var tran = cnx.BeginTransaction())
				{
					using (var cmd = cnx.CreateCommand())
					{
						cmd.Transaction = tran;
						cmd.CommandType = CommandType.Text;
						cmd.CommandText = "insert into test (id) values (1)";
						cmd.ExecuteNonQuery();
					}
					tran.Commit();
				}
			}

			using (var cnx = new FbConnection(cnxStr))
			{
				cnx.Open();
				using (var tran = cnx.BeginTransaction())
				{
					using (var cmd = cnx.CreateCommand())
					{
						cmd.Transaction = tran;
						cmd.CommandType = CommandType.Text;
						cmd.CommandText = "select id from test";
						using (var reader = cmd.ExecuteReader())
						{
							Assert.IsTrue(reader.Read());
							Assert.AreEqual(1, reader.GetInt32(0));
							Assert.IsFalse(reader.Read());
						}
					}
					tran.Commit();
				}
			}

			using (var cnx = new FbConnection(cnxStr))
			{
				cnx.Open();
				using (var tran = cnx.BeginTransaction())
				{
					using (var cmd = cnx.CreateCommand())
					{
						cmd.Transaction = tran;
						cmd.CommandType = CommandType.Text;
						cmd.CommandText = "delete from test";
						cmd.ExecuteNonQuery();
					}
					tran.Commit();
				}
			}

			using (var cnx = new FbConnection(cnxStr))
			{
				//FbConnection.ClearPool(cnx);
			}

			using (var cnx = new FbConnection(cnxStr))
			{
				cnx.Open();
				using (var tran = cnx.BeginTransaction())
				{
					using (var cmd = cnx.CreateCommand())
					{
						cmd.Transaction = tran;
						cmd.CommandType = CommandType.Text;
						cmd.CommandText = "drop table test";
						cmd.ExecuteNonQuery();
					}
					tran.Commit();
				}
			}
		}

		[Test]
		public void Drop()
		{
			var cfg = TestConfigurationHelper.GetDefaultConfiguration();
			var cnxStr = cfg.Properties[Environment.ConnectionString];
			using (var cnx = new FbConnection(cnxStr))
			{
				cnx.Open();
				using (var tran = cnx.BeginTransaction())
				{
					using (var cmd = cnx.CreateCommand())
					{
						cmd.Transaction = tran;
						cmd.CommandType = CommandType.Text;
						cmd.CommandText = "drop table test";
						cmd.ExecuteNonQuery();
					}
					tran.Commit();
				}
			}
		}

		[Test]
		public void GetLimitString()
		{
			var str = _dialect.GetLimitString(new SqlString("SELECT * FROM fish"), null, new SqlString("10"));
			Assert.AreEqual("SELECT first 10 * FROM fish", str.ToString());

			str = _dialect.GetLimitString(new SqlString("SELECT * FROM fish ORDER BY name"), new SqlString("5"), new SqlString("15"));
			Assert.AreEqual("SELECT first 15 skip 5 * FROM fish ORDER BY name", str.ToString());

			str = _dialect.GetLimitString(new SqlString("SELECT * FROM fish ORDER BY name DESC"), new SqlString("7"),
				new SqlString("28"));
			Assert.AreEqual("SELECT first 28 skip 7 * FROM fish ORDER BY name DESC", str.ToString());

			str = _dialect.GetLimitString(new SqlString("SELECT DISTINCT fish.family FROM fish ORDER BY name DESC"), null,
				new SqlString("28"));
			Assert.AreEqual("SELECT first 28 DISTINCT fish.family FROM fish ORDER BY name DESC", str.ToString());

			str = _dialect.GetLimitString(new SqlString("SELECT DISTINCT fish.family FROM fish ORDER BY name DESC"), new SqlString("7"),
				new SqlString("28"));
			Assert.AreEqual("SELECT first 28 skip 7 DISTINCT fish.family FROM fish ORDER BY name DESC", str.ToString());
		}

		[Test]
		public void GetTypeName_DecimalWithoutPrecisionAndScale_ReturnsDecimalWithPrecisionOf18AndScaleOf5()
		{
			var result = _dialect.GetTypeName(NHibernateUtil.Decimal.SqlType);

			Assert.AreEqual("DECIMAL(18, 5)", result);
		}

		[Test]
		public void GetTypeName_DecimalWithPrecisionAndScale_ReturnsPrecisedAndScaledDecimal()
		{
			var result = _dialect.GetTypeName(NHibernateUtil.Decimal.SqlType, 0, 17, 2);

			Assert.AreEqual("DECIMAL(17, 2)", result);
		}

		[Test]
		public void GetTypeName_DecimalWithPrecisionGreaterThanFbMaxPrecision_ReturnsDecimalWithFbMaxPrecision()
		{
			var result = _dialect.GetTypeName(NHibernateUtil.Decimal.SqlType, 0, 19, 2);
				//Firebird allows a maximum precision of 18

			Assert.AreEqual("DECIMAL(18, 2)", result);
		}

		#region Private Members

		#endregion
	}
}
