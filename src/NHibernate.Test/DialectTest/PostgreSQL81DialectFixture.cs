﻿using System.Data;
using NHibernate.Dialect;
using NHibernate.SqlTypes;
using NUnit.Framework;

namespace NHibernate.Test.DialectTest
{
	[TestFixture]
	public class PostgreSQL81DialectFixture
	{
		[Test]
		public void ScaleTypes()
		{
			const int min = 0;
			const int intermediate = 4;
			const int max = 6;
			var dialect = new PostgreSQL81Dialect();

			Assert.That(dialect.GetTypeName(SqlTypeFactory.DateTime), Is.EqualTo("timestamp").IgnoreCase, "Default datetime");
			Assert.That(dialect.GetTypeName(SqlTypeFactory.GetDateTime(min)), Is.EqualTo($"timestamp({min})").IgnoreCase, "Min datetime");
			Assert.That(dialect.GetTypeName(SqlTypeFactory.GetDateTime(intermediate)), Is.EqualTo($"timestamp({intermediate})").IgnoreCase, "Intermediate datetime");
			Assert.That(dialect.GetTypeName(SqlTypeFactory.GetDateTime(max)), Is.EqualTo($"timestamp({max})").IgnoreCase, "Max datetime");
			Assert.That(dialect.GetLongestTypeName(DbType.DateTime), Is.EqualTo($"timestamp({max})").IgnoreCase, "Longest datetime");
			Assert.That(dialect.GetTypeName(SqlTypeFactory.GetDateTime(max + 1)), Is.EqualTo("timestamp").IgnoreCase, "Over max datetime");

			Assert.That(dialect.GetTypeName(SqlTypeFactory.Time), Is.EqualTo($"time").IgnoreCase, "Default time");
			Assert.That(dialect.GetTypeName(SqlTypeFactory.GetTime(min)), Is.EqualTo($"time({min})").IgnoreCase, "Min time");
			Assert.That(dialect.GetTypeName(SqlTypeFactory.GetTime(intermediate)), Is.EqualTo($"time({intermediate})").IgnoreCase, "Intermediate time");
			Assert.That(dialect.GetTypeName(SqlTypeFactory.GetTime(max)), Is.EqualTo($"time({max})").IgnoreCase, "Max time");
			Assert.That(dialect.GetLongestTypeName(DbType.Time), Is.EqualTo($"time({max})").IgnoreCase, "Longest time");
			Assert.That(dialect.GetTypeName(SqlTypeFactory.GetTime(max + 1)), Is.EqualTo($"time").IgnoreCase, "Over max time");
		}
	}
}
