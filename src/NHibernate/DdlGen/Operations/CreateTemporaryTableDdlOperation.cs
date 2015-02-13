using System.Collections.Generic;
using System.Text;
using NHibernate.DdlGen.Model;
using NHibernate.Util;

namespace NHibernate.DdlGen.Operations
{
    public class CreateTemporaryTableDdlOperation : IDdlOperation
    {
        private readonly CreateTemporaryTableModel _model;

        public CreateTemporaryTableDdlOperation(CreateTemporaryTableModel model)
        {
            _model = model;
        }
        public IEnumerable<string> GetStatements(Dialect.Dialect dialect)
        {
            var sb = new StringBuilder();
            sb.Append(dialect.CreateTemporaryTableString).Append(" ")
                .Append(BacktickQuoteUtil.ApplyDialectQuotes(_model.Name.Name, dialect))
                .Append(" (");
            bool needsComma = false;
            foreach (var c in _model.Columns)
            {
                if (needsComma)
                {
                    sb.Append(", ");
                }
                needsComma = true;
                sb = c.AppendTemporaryTableColumnDefinition(sb, dialect);
            }
            sb.Append(") ")
                .Append(dialect.CreateTemporaryTablePostfix);
            var result = sb.ToString();
            return new[] {result};

        }
    }
}