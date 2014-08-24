using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Test.TestDialects
{
	public class SQLiteTestDialect : TestDialect
	{
        public SQLiteTestDialect(Dialect.Dialect dialect)
            : base(dialect)
        {
        }

		public override bool SupportsOperatorAll
		{
			get { return false; }
		}

		public override bool SupportsOperatorSome
		{
			get { return false; }
		}

		public override bool SupportsLocate
		{
			get { return false; }
		}

		public override bool SupportsDistributedTransactions
		{
			get { return false; }
		}

		public override bool SupportsConcurrentTransactions
		{
			get { return false; }
		}

		public override bool SupportsFullJoin
		{
			get { return false; }
		}

        public override bool HasBrokenDecimalType
        {
            get { return true; }
        }

        public override bool SupportsHavingWithoutGroupBy
        {
            get { return false; }
        }
	}
}
