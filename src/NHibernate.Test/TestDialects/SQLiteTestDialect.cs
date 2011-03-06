using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Test.TestDialects
{
	public class SQLiteTestDialect : TestDialect
	{
		public override bool SupportsOperatorAll
		{
			get { return false; }
		}

		public override bool SupportsOperatorSome
		{
			get { return false; }
		}
	}
}
