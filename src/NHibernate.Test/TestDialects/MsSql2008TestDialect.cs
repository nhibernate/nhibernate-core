using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Test.TestDialects
{
	public class MsSql2008TestDialect : TestDialect
	{
        public MsSql2008TestDialect(Dialect.Dialect dialect)
            : base(dialect)
        {
        }

        public override bool IgnoresTrailingWhitespace
        {
            get { return true; }
        }
	}
}
