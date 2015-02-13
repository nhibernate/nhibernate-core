using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.DdlGen.Operations
{
    public class AggregateDdlOperation : IDdlOperation
    {
        private readonly IDdlOperation[] _componentDdlOperations;

        public AggregateDdlOperation(params IDdlOperation[] componentDdlOperations)
        {
            _componentDdlOperations = componentDdlOperations;
        }
        public IEnumerable<string> GetStatements(Dialect.Dialect dialect)
        {
            return _componentDdlOperations.SelectMany(o => o.GetStatements(dialect));
        }
    }
}
