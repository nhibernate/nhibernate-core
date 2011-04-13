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
	}
}
