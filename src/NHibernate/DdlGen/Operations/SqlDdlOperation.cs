using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.DdlGen.Operations
{
    public class SqlDdlOperation : IDdlOperation
    {
        private readonly string[] _sqlsStrings;

        public SqlDdlOperation(params string [] sqlsStrings)
        {
            _sqlsStrings = sqlsStrings;
        }
        public IEnumerable<string> GetStatements(Dialect.Dialect dialect)
        {
            return _sqlsStrings;
        }
    }
}
