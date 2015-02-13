using System.Collections.Generic;

namespace NHibernate.DdlGen.Operations
{
    public class DisableForeignKeyConstraintDdlOperation : IDdlOperation
    {
        public IEnumerable<string> GetStatements(Dialect.Dialect dialect)
        {
            yield return dialect.DisableForeignKeyConstraintsString;
        }
    }
}