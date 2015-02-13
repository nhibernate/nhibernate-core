using System.Collections.Generic;

namespace NHibernate.DdlGen.Operations
{
    public class DropSequenceDdlOperation : IDdlOperation
    {
        private readonly string _name;

        public DropSequenceDdlOperation(string name)
        {
            _name = name;
        }

        public IEnumerable<string> GetStatements(Dialect.Dialect dialect)
        {
            return new[]{dialect.GetDropSequenceString(_name)};
        }
    }
}