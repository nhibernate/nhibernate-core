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

		private static readonly FieldInfo _mappingField =
			typeof(Configuration).GetField("mapping", BindingFlags.Instance | BindingFlags.NonPublic);

		private static IMapping GetMapping(Configuration cfg)
		{
			Assert.That(_mappingField, Is.Not.Null, "Unable to find field mapping");
			var mapping = _mappingField.GetValue(cfg) as IMapping;
			Assert.That(mapping, Is.Not.Null, "Unable to find mapping object");
			return mapping;
		}

		private static void AssertSqlType(IType type, SqlType sqlType, IMapping mapping)
		{
			Assert.That(type.SqlTypes(mapping), Has.Length.EqualTo(1).And.Contain(sqlType));
		}
	}
}
