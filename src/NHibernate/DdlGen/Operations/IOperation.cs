using System.Collections.Generic;

namespace NHibernate.DdlGen.Operations
{
    public interface IDdlOperation
    {
        IEnumerable<string> GetStatements(NHibernate.Dialect.Dialect dialect);
    }
}
