using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
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
			string testDialectTypeName = "NHibernate.Test.TestDialects." + dialect.GetType().Name.Replace("Dialect", "TestDialect");
			System.Type testDialectType = System.Type.GetType(testDialectTypeName);
			if (testDialectType != null)
				return (TestDialect)Activator.CreateInstance(testDialectType, dialect);
			return new TestDialect(dialect);
		}

	    private Dialect.Dialect dialect;

        public TestDialect(Dialect.Dialect dialect)
        {
            this.dialect = dialect;
        }

		public virtual bool SupportsOperatorAll { get { return true; } }
		public virtual bool SupportsOperatorSome { get { return true; } }
		public virtual bool SupportsLocate { get { return true; } }

		public virtual bool SupportsDistributedTransactions { get { return true; } }

		/// <summary>
		/// Whether two transactions can be run at the same time.  For example, with SQLite
		/// the database is locked when one transaction is run, so running a second transaction
		/// will cause a "database is locked" error message.
		/// </summary>
		public virtual bool SupportsConcurrentTransactions { get { return true; } }

		public virtual bool SupportsFullJoin { get { return true; } }

        public virtual bool HasBrokenDecimalType { get { return false; } }

        public virtual bool SupportsNullCharactersInUtfStrings { get { return true; } }

        public virtual bool SupportsSelectForUpdateOnOuterJoin { get { return true; } }

        public virtual bool SupportsHavingWithoutGroupBy { get { return true; } }

        public virtual bool IgnoresTrailingWhitespace { get { return false; } }

	    public bool SupportsSqlType(SqlType sqlType)
	    {
            try
            {
                dialect.GetTypeName(sqlType);
                return true;
            }
            catch
            {
                return false;
            }
	    }
	}
}
