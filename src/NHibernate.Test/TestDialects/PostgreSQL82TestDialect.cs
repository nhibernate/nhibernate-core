using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Test.TestDialects
{
	public class PostgreSQL82TestDialect : TestDialect
	{
        public PostgreSQL82TestDialect(Dialect.Dialect dialect)
            : base(dialect)
        {
        }

        public override bool SupportsSelectForUpdateOnOuterJoin
        {
            get { return false; }
        }

        public override bool SupportsNullCharactersInUtfStrings
        {
            get { return false; }
        }

        /// <summary>
        /// Npgsql's DTC code seems to be somewhat broken as of 2.0.11.
        /// </summary>
        public override bool SupportsDistributedTransactions
        {
            get { return false; }
        }
	}
}
