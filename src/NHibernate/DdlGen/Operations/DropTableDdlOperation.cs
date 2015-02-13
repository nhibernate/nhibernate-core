using System.Collections.Generic;
using NHibernate.DdlGen.Model;

namespace NHibernate.DdlGen.Operations
{
    public class DropTableDdlOperation : IDdlOperation
    {
        private readonly DbName _tableName;

        public DropTableDdlOperation(DbName name)
        {
            _tableName = name;
        }

        public DbName TableName
        {
            get { return _tableName; }
        }

        public IEnumerable<string> GetStatements(Dialect.Dialect dialect)
        {
            var tableName = TableName.QuoteAndQualify(dialect);
            return new[]
            {
                dialect.GetDropTableString(tableName)
            };
        }
    }
}