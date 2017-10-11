using System;
using System.Collections;
using System.Data;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NUnit.Framework;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Test.DriverTest
{
	public class MultiTypeEntity
	{
		public virtual int Id { get; set; }
		public virtual string StringProp { get; set; }
		public virtual string AnsiStringProp { get; set; }
		public virtual decimal Decimal { get; set; }
		public virtual decimal DecimalHighScale { get; set; }
		public virtual decimal Currency { get; set; }
		public virtual double Double { get; set; }
		public virtual float Float { get; set; }
		public virtual byte[] BinaryBlob { get; set; }
		public virtual byte[] Binary { get; set; }
		public virtual string StringClob { get; set; }
		public virtual DateTime DateTimeProp { get; set; } = DateTime.Now;
	}

	[TestFixture]
	public class SqlClientDriverFixture : TestCase
	{
		protected override string MappingsAssembly => "NHibernate.Test";

		protected override IList Mappings => new[] { "DriverTest.MultiTypeEntity.hbm.xml" };

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is MsSql2008Dialect;
		}

		protected override bool AppliesTo(ISessionFactoryImplementor factory)
		{
			return factory.ConnectionProvider.Driver.IsSqlClientDriver();
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.CreateQuery("delete from MultiTypeEntity").ExecuteUpdate();
				t.Commit();
			}
		}

		[Test]
		public void Crud()
		{
			// Should use default dimension for CRUD op because the mapping does not 
			// have dimensions specified.
			object savedId;
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				savedId = s.Save(
					new MultiTypeEntity
					{
						StringProp = "a",
						StringClob = "a",
						BinaryBlob = new byte[] { 1, 2, 3 },
						Binary = new byte[] { 4, 5, 6 },
						Currency = 123.4m,
						Double = 123.5d,
						Decimal = 789.5m,
						DecimalHighScale = 1234567890.0123456789m
					});
				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var m = s.Get<MultiTypeEntity>(savedId);

				Assert.That(m.StringProp, Is.EqualTo("a"), "StringProp");
				Assert.That(m.StringClob, Is.EqualTo("a"), "StringClob");
				Assert.That(m.BinaryBlob, Is.EqualTo(new byte[] { 1, 2, 3 }), "BinaryBlob");
				Assert.That(m.Binary, Is.EqualTo(new byte[] { 4, 5, 6 }), "BinaryBlob");
				Assert.That(m.Currency, Is.EqualTo(123.4m), "Currency");
				Assert.That(m.Double, Is.EqualTo(123.5d).Within(0.0001d), "Double");
				Assert.That(m.Decimal, Is.EqualTo(789.5m), "Decimal");
				Assert.That(m.DecimalHighScale, Is.EqualTo(1234567890.0123456789m), "DecimalHighScale");

				m.StringProp = "b";
				m.StringClob = "b";
				m.BinaryBlob = new byte[] { 4, 5, 6 };
				m.Binary = new byte[] { 7, 8, 9 };
				m.Currency = 456.78m;
				m.Double = 987.6d;
				m.Decimal = 1323456.45m;
				m.DecimalHighScale = 9876543210.0123456789m;
				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var m = s.Load<MultiTypeEntity>(savedId);

				Assert.That(m.StringProp, Is.EqualTo("b"), "StringProp");
				Assert.That(m.StringClob, Is.EqualTo("b"), "StringClob");
				Assert.That(m.BinaryBlob, Is.EqualTo(new byte[] { 4, 5, 6 }), "BinaryBlob");
				Assert.That(m.Binary, Is.EqualTo(new byte[] { 7, 8, 9 }), "BinaryBlob");
				Assert.That(m.Currency, Is.EqualTo(456.78m), "Currency");
				Assert.That(m.Double, Is.EqualTo(987.6d).Within(0.0001d), "Double");
				Assert.That(m.Decimal, Is.EqualTo(1323456.45m), "Decimal");
				Assert.That(m.DecimalHighScale, Is.EqualTo(9876543210.0123456789m), "DecimalHighScale");

				t.Commit();
			}
		}

		[Test]
		public void QueryPlansAreReused()
		{
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				// clear the existing plan cache
				s.CreateSQLQuery("DBCC FREEPROCCACHE").ExecuteUpdate();
				t.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				var countPlansCommand = s.CreateSQLQuery("SELECT COUNT(*) FROM sys.dm_exec_cached_plans");

				var beforeCount = countPlansCommand.UniqueResult<int>();

				var insertCount = 10;
				for (var i = 0; i < insertCount; i++)
				{
					s.Save(new MultiTypeEntity() { StringProp = new string('x', i + 1) });
					s.Flush();
				}

				var afterCount = countPlansCommand.UniqueResult<int>();

				Assert.That(
					afterCount - beforeCount,
					Is.LessThan(insertCount - 1),
					$"Excessive query plans created: before={beforeCount} after={afterCount}");

				t.Rollback();
			}
		}

		[Test]
		public void DefaultPrecisionScale()
		{
			const byte defaultPrecision = 28;
			const byte defaultScale = 10;
			var driver = Sfi.ConnectionProvider.Driver;
			try
			{
				using (var cmd = new System.Data.SqlClient.SqlCommand())
				{
					var p = driver.GenerateParameter(cmd, "p", SqlTypeFactory.Decimal);
					Assert.That(p.Precision, Is.EqualTo(defaultPrecision), "no defaults");
					Assert.That(p.Scale, Is.EqualTo(defaultScale), "no defaults");
					p = driver.GenerateParameter(cmd, "p", SqlTypeFactory.GetSqlType(DbType.Decimal, 24, 11));
					Assert.That(p.Precision, Is.EqualTo(24), "explicit without defaults");
					Assert.That(p.Scale, Is.EqualTo(11), "explicit without defaults");

					var configuration = TestConfigurationHelper.GetDefaultConfiguration();
					configuration.SetProperty(Environment.QueryDefaultCastPrecision, "26");
					driver.Configure(configuration.Properties);
					p = driver.GenerateParameter(cmd, "p", SqlTypeFactory.Decimal);
					Assert.That(p.Precision, Is.EqualTo(26), "default precision 26");
					Assert.That(p.Scale, Is.EqualTo(defaultScale), "default precision 26");
					p = driver.GenerateParameter(cmd, "p", SqlTypeFactory.GetSqlType(DbType.Decimal, 24, 11));
					Assert.That(p.Precision, Is.EqualTo(24), "explicit 24 with default precision 26");
					Assert.That(p.Scale, Is.EqualTo(11), "explicit 24 with default precision 26");

					configuration.Properties.Remove(Environment.QueryDefaultCastPrecision);
					configuration.SetProperty(Environment.QueryDefaultCastScale, "8");
					driver.Configure(configuration.Properties);
					p = driver.GenerateParameter(cmd, "p", SqlTypeFactory.Decimal);
					Assert.That(p.Precision, Is.EqualTo(defaultPrecision), "default scale 8");
					Assert.That(p.Scale, Is.EqualTo(8), "default scale 8");
					p = driver.GenerateParameter(cmd, "p", SqlTypeFactory.GetSqlType(DbType.Decimal, 24, 11));
					Assert.That(p.Precision, Is.EqualTo(24), "explicit with default scale 8");
					Assert.That(p.Scale, Is.EqualTo(11), "explicit with default scale 8");

					configuration.SetProperty(Environment.QueryDefaultCastPrecision, "34");
					configuration.SetProperty(Environment.QueryDefaultCastScale, "15");
					driver.Configure(configuration.Properties);
					p = driver.GenerateParameter(cmd, "p", SqlTypeFactory.Decimal);
					Assert.That(p.Precision, Is.EqualTo(34), "default 34,15");
					Assert.That(p.Scale, Is.EqualTo(15), "default 34,15");
					p = driver.GenerateParameter(cmd, "p", SqlTypeFactory.GetSqlType(DbType.Decimal, 24, 11));
					Assert.That(p.Precision, Is.EqualTo(24), "explicit with default 34,15");
					Assert.That(p.Scale, Is.EqualTo(11), "explicit with default 34,15");
				}
			}
			finally
			{
				driver.Configure(cfg.GetDerivedProperties());
			}
		}
	}
}
