﻿using System;
using NHibernate.SqlTypes;

namespace NHibernate.Test
{
	/// <summary>
	/// Like NHibernate's Dialect class, but for differences only important during testing.
	/// Defaults to true for all support.  Users of different dialects can turn support
	/// off if the unit tests fail.
	/// </summary>
	public class TestDialect
	{
		public static TestDialect GetTestDialect(Dialect.Dialect dialect)
		{
			var testDialectTypeName = "NHibernate.Test.TestDialects." + dialect.GetType().Name.Replace("Dialect", "TestDialect");
			var testDialectType = System.Type.GetType(testDialectTypeName);
			if (testDialectType != null)
				return (TestDialect) Activator.CreateInstance(testDialectType, dialect);
			return new TestDialect(dialect);
		}

		readonly Dialect.Dialect _dialect;

		public TestDialect(Dialect.Dialect dialect)
		{
			_dialect = dialect;
		}

		public virtual bool SupportsOperatorAll => true;
		public virtual bool SupportsOperatorSome => true;
		public virtual bool SupportsLocate => true;

		public virtual bool SupportsDistributedTransactions => true;

		/// <summary>
		/// Whether two transactions can be run at the same time.  For example, with SQLite
		/// the database is locked when one transaction is run, so running a second transaction
		/// will cause a "database is locked" error message.
		/// </summary>
		public virtual bool SupportsConcurrentTransactions => true;

		public virtual bool SupportsFullJoin => true;

		public virtual bool HasBrokenDecimalType => false;

		public virtual bool SupportsNullCharactersInUtfStrings => true;

		public virtual bool SupportsSelectForUpdateOnOuterJoin => true;

		public virtual bool SupportsHavingWithoutGroupBy => true;

		public virtual bool SupportsComplexExpressionInGroupBy => true;

		public virtual bool SupportsCountDistinct => true;

		public virtual bool SupportsOrderByAggregate => true;

		public virtual bool SupportsOrderByColumnNumber => true;

		public virtual bool SupportsDuplicatedColumnAliases => true;

		/// <summary>
		/// Supports inserting in a table without any column specified in the insert.
		/// </summary>
		public virtual bool SupportsEmptyInserts => true;

		public bool SupportsSqlType(SqlType sqlType)
		{
			try
			{
				_dialect.GetTypeName(sqlType);
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}
