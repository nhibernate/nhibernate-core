using System.Collections.Generic;

namespace NHibernate.DdlGen.Operations
{
    public class DropSequenceDdlOperation : IDdlOperation
    {
        public string Name { get; private set; }

        public DropSequenceDdlOperation(string name)
        {
            Name = name;
        }

        public IEnumerable<string> GetStatements(Dialect.Dialect dialect)
        {
            return new[]{dialect.GetDropSequenceString(Name)};
        }

    }
}