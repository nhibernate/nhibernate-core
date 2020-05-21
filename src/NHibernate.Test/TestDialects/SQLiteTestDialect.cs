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

		public override bool SupportsLocateStartIndex
		{
			get { return false; }
		}

		public override bool SupportsFullJoin
		{
			get { return false; }
		}

		/// <summary>
		/// SqlLite stores them as float instead.
		/// </summary>
		public override bool HasBrokenDecimalType
		{
			get { return true; }
		}

		public override bool SupportsHavingWithoutGroupBy
		{
			get { return false; }
		}

		public override bool SupportsModuloOnDecimal => false;

		/// <summary>
		/// Does not support update locks
		/// </summary>
		public override bool SupportsSelectForUpdate => false;

		public override bool SupportsAggregateInSubSelect => true;
	}
}
