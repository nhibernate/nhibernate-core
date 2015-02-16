using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace NHibernate.DdlGen.Operations
{
    public interface IDdlOperation
    {
        
        IEnumerable<string> GetStatements(NHibernate.Dialect.Dialect dialect);
    }
}
