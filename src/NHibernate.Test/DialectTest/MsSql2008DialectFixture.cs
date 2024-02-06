using System.Data;
using System.Reflection;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NUnit.Framework;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Test.DialectTest
{
	[TestFixture]
	public class MsSql2008DialectFixture
	{
		[Test]
		public void CheckPreSql2008DateTimeTypes()
		{
			var cfg = TestConfigurationHelper.GetDefaultConfiguration();
			cfg.SetProperty(Environment.Dialect, typeof(MsSql2005Dialect).FullName);
			var mapping = GetMapping(cfg);
			AssertSqlType(NHibernateUtil.DateTime, SqlTypeFactory.DateTime, mapping);
#pragma warning disable 618 // Timestamp is obsolete
			AssertSqlType(NHibernateUtil.Timestamp, SqlTypeFactory.DateTime, mapping);
#pragma warning restore 618
			AssertSqlType(NHibernateUtil.DbTimestamp, SqlTypeFactory.DateTime, mapping);
			AssertSqlType(NHibernateUtil.LocalDateTime, SqlTypeFactory.DateTime, mapping);
			AssertSqlType(NHibernateUtil.UtcDateTime, SqlTypeFactory.DateTime, mapping);
#pragma warning disable 618 // DateTime2 is obsolete
			AssertSqlType(NHibernateUtil.DateTime2, SqlTypeFactory.DateTime2, mapping);
#pragma warning restore 618
		}

		[Test]
		public void CheckPreSql2008DateTimeTypesWithScale()
		{
			var cfg = TestConfigurationHelper.GetDefaultConfiguration();
			cfg.SetProperty(Environment.Dialect, typeof(MsSql2005Dialect).FullName);
			var mapping = GetMapping(cfg);
			AssertSqlType(TypeFactory.GetDateTimeType(0), SqlTypeFactory.GetDateTime(0), mapping);
			AssertSqlType(TypeFactory.GetLocalDateTimeType(1), SqlTypeFactory.GetDateTime(1), mapping);
			AssertSqlType(TypeFactory.GetUtcDateTimeType(2), SqlTypeFactory.GetDateTime(2), mapping);
#pragma warning disable 618 // DateTime2 is obsolete
			AssertSqlType(TypeFactory.GetDateTime2Type(3), SqlTypeFactory.GetDateTime2(3), mapping);
#pragma warning restore 618
		}

		[Test]
		public void CheckSql2008DateTimeTypes()
		{
			var cfg = TestConfigurationHelper.GetDefaultConfiguration();
			cfg.SetProperty(Environment.Dialect, typeof(MsSql2008Dialect).FullName);
			var mapping = GetMapping(cfg);
			AssertSqlType(NHibernateUtil.DateTime, SqlTypeFactory.DateTime2, mapping);
#pragma warning disable 618 // Timestamp is obsolete
			AssertSqlType(NHibernateUtil.Timestamp, SqlTypeFactory.DateTime2, mapping);
#pragma warning restore 618
			AssertSqlType(NHibernateUtil.DbTimestamp, SqlTypeFactory.DateTime2, mapping);
			AssertSqlType(NHibernateUtil.LocalDateTime, SqlTypeFactory.DateTime2, mapping);
			AssertSqlType(NHibernateUtil.UtcDateTime, SqlTypeFactory.DateTime2, mapping);
#pragma warning disable 618 // DateTime2 is obsolete
			AssertSqlType(NHibernateUtil.DateTime2, SqlTypeFactory.DateTime2, mapping);
#pragma warning restore 618
		}

		[Test]
		public void CheckSql2008DateTimeTypesWithScale()
		{
			var cfg = TestConfigurationHelper.GetDefaultConfiguration();
			cfg.SetProperty(Environment.Dialect, typeof(MsSql2008Dialect).FullName);
			var mapping = GetMapping(cfg);
			AssertSqlType(TypeFactory.GetDateTimeType(4), SqlTypeFactory.GetDateTime2(4), mapping);
			AssertSqlType(TypeFactory.GetLocalDateTimeType(5), SqlTypeFactory.GetDateTime2(5), mapping);
			AssertSqlType(TypeFactory.GetUtcDateTimeType(6), SqlTypeFactory.GetDateTime2(6), mapping);
#pragma warning disable 618 // DateTime2 is obsolete
			AssertSqlType(TypeFactory.GetDateTime2Type(7), SqlTypeFactory.GetDateTime2(7), mapping);
#pragma warning restore 618
		}

		[Test]
		public void CheckKeepDateTime()
		{
			var cfg = TestConfigurationHelper.GetDefaultConfiguration();
			cfg.SetProperty(Environment.Dialect, typeof(MsSql2008Dialect).FullName);
			cfg.SetProperty(Environment.SqlTypesKeepDateTime, "true");
			var mapping = GetMapping(cfg);
			AssertSqlType(NHibernateUtil.DateTime, SqlTypeFactory.DateTime, mapping);
#pragma warning disable 618 // Timestamp is obsolete
			AssertSqlType(NHibernateUtil.Timestamp, SqlTypeFactory.DateTime, mapping);
#pragma warning restore 618
			AssertSqlType(NHibernateUtil.DbTimestamp, SqlTypeFactory.DateTime, mapping);
			AssertSqlType(NHibernateUtil.LocalDateTime, SqlTypeFactory.DateTime, mapping);
			AssertSqlType(NHibernateUtil.UtcDateTime, SqlTypeFactory.DateTime, mapping);
#pragma warning disable 618 // DateTime2 is obsolete
			AssertSqlType(NHibernateUtil.DateTime2, SqlTypeFactory.DateTime2, mapping);
#pragma warning restore 618
		}

		[Test]
		public void CheckKeepDateTimeWithScale()
		{
			var cfg = TestConfigurationHelper.GetDefaultConfiguration();
			cfg.SetProperty(Environment.Dialect, typeof(MsSql2008Dialect).FullName);
			cfg.SetProperty(Environment.SqlTypesKeepDateTime, "true");
			var mapping = GetMapping(cfg);
			AssertSqlType(TypeFactory.GetDateTimeType(0), SqlTypeFactory.GetDateTime(0), mapping);
			AssertSqlType(TypeFactory.GetLocalDateTimeType(1), SqlTypeFactory.GetDateTime(1), mapping);
			AssertSqlType(TypeFactory.GetUtcDateTimeType(2), SqlTypeFactory.GetDateTime(2), mapping);
#pragma warning disable 618 // DateTime2 is obsolete
			AssertSqlType(TypeFactory.GetDateTime2Type(3), SqlTypeFactory.GetDateTime2(3), mapping);
#pragma warning restore 618
		}

		[Test]
		public void ScaleTypes()
		{
			const int min = 0;
			const int intermediate = 5;
			const int max = 7;
			var dialect = new MsSql2008Dialect();

			Assert.That(dialect.GetTypeName(SqlTypeFactory.DateTime), Is.EqualTo("datetime").IgnoreCase, "Default datetime");
			Assert.That(dialect.GetTypeName(SqlTypeFactory.GetDateTime(min)), Is.EqualTo("datetime").IgnoreCase, "Min datetime");
			Assert.That(dialect.GetTypeName(SqlTypeFactory.GetDateTime(intermediate)), Is.EqualTo("datetime").IgnoreCase, "Intermediate datetime");
			Assert.That(dialect.GetTypeName(SqlTypeFactory.GetDateTime(max)), Is.EqualTo("datetime").IgnoreCase, "Max datetime");
			Assert.That(dialect.GetLongestTypeName(DbType.DateTime), Is.EqualTo("datetime").IgnoreCase, "Longest datetime");
			Assert.That(dialect.GetTypeName(SqlTypeFactory.GetDateTime(max + 1)), Is.EqualTo("datetime").IgnoreCase, "Over max datetime");

			Assert.That(dialect.GetTypeName(SqlTypeFactory.DateTime2), Is.EqualTo("datetime2").IgnoreCase, "Default datetime2");
			Assert.That(dialect.GetTypeName(SqlTypeFactory.GetDateTime2(min)), Is.EqualTo($"datetime2({min})").IgnoreCase, "Min datetime2");
			Assert.That(dialect.GetTypeName(SqlTypeFactory.GetDateTime2(intermediate)), Is.EqualTo($"datetime2({intermediate})").IgnoreCase, "Intermediate datetime2");
			Assert.That(dialect.GetTypeName(SqlTypeFactory.GetDateTime2(max)), Is.EqualTo($"datetime2({max})").IgnoreCase, "Max datetime2");
			Assert.That(dialect.GetLongestTypeName(DbType.DateTime2), Is.EqualTo($"datetime2({max})").IgnoreCase, "Longest datetime2");
			Assert.That(dialect.GetTypeName(SqlTypeFactory.GetDateTime2(max + 1)), Is.EqualTo("datetime2").IgnoreCase, "Over max datetime2");

			Assert.That(dialect.GetTypeName(SqlTypeFactory.DateTimeOffSet), Is.EqualTo("datetimeoffset").IgnoreCase, "Default datetimeoffset");
			Assert.That(dialect.GetTypeName(SqlTypeFactory.GetDateTimeOffset(min)), Is.EqualTo($"datetimeoffset({min})").IgnoreCase, "Min datetimeoffset");
			Assert.That(dialect.GetTypeName(SqlTypeFactory.GetDateTimeOffset(intermediate)), Is.EqualTo($"datetimeoffset({intermediate})").IgnoreCase, "Intermediate datetimeoffset");
			Assert.That(dialect.GetTypeName(SqlTypeFactory.GetDateTimeOffset(max)), Is.EqualTo($"datetimeoffset({max})").IgnoreCase, "Max datetimeoffset");
			Assert.That(dialect.GetLongestTypeName(DbType.DateTimeOffset), Is.EqualTo($"datetimeoffset({max})").IgnoreCase, "Longest datetimeoffset");
			Assert.That(dialect.GetTypeName(SqlTypeFactory.GetDateTimeOffset(max + 1)), Is.EqualTo("datetimeoffset").IgnoreCase, "Over max datetimeoffset");

			Assert.That(dialect.GetTypeName(SqlTypeFactory.Time), Is.EqualTo("time").IgnoreCase, "Default time");
			Assert.That(dialect.GetTypeName(SqlTypeFactory.GetTime(min)), Is.EqualTo($"time({min})").IgnoreCase, "Min time");
			Assert.That(dialect.GetTypeName(SqlTypeFactory.GetTime(intermediate)), Is.EqualTo($"time({intermediate})").IgnoreCase, "Intermediate time");
			Assert.That(dialect.GetTypeName(SqlTypeFactory.GetTime(max)), Is.EqualTo($"time({max})").IgnoreCase, "Max time");
			Assert.That(dialect.GetLongestTypeName(DbType.Time), Is.EqualTo($"time({max})").IgnoreCase, "Longest time");
			Assert.That(dialect.GetTypeName(SqlTypeFactory.GetTime(max + 1)), Is.EqualTo("time").IgnoreCase, "Over max time");
		}

		private static IMapping GetMapping(Configuration cfg) => (IMapping) cfg.BuildSessionFactory();

		private static void AssertSqlType(IType type, SqlType sqlType, IMapping mapping)
		{
			Assert.That(type.SqlTypes(mapping), Has.Length.EqualTo(1).And.Contain(sqlType), type.Name);
		}
	}
}
