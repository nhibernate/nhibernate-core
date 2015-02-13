using System;
using System.Collections.Generic;
using NHibernate.DdlGen.Model;

namespace NHibernate.DdlGen.Operations
{
    public class CreateSequenceDdlOperation : IDdlOperation
    {
        public CreateSequenceModel Model { get; private set; }

        public CreateSequenceDdlOperation(CreateSequenceModel model)
        {
            Model = model;
        }

        public IEnumerable<string> GetStatements(Dialect.Dialect dialect)
        {
            if (Model.InitialValue > 1 || Model.IncrementSize > 1)
            {
                return dialect.GetCreateSequenceStrings(Model.Name.QuoteAndQualify(dialect), Model.InitialValue, Model.IncrementSize);
            }
            string baseDDL = dialect.GetCreateSequenceString(Model.Name.QuoteAndQualify(dialect));
            string paramsDDL = null;
            if (!String.IsNullOrWhiteSpace(Model.Parameters))
            {
                paramsDDL = ' ' + Model.Parameters;
            }
            return new[] {string.Concat(baseDDL, paramsDDL)};
        }
    }
}