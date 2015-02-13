using System.Collections.Generic;

namespace NHibernate.DdlGen.Operations
{
    public class EnableForeignKeyConstratintDdlOperation : IDdlOperation
    {
        public IEnumerable<string> GetStatements(Dialect.Dialect dialect)
        {
            yield return dialect.EnableForeignKeyConstraintsString;
        }
    }
}