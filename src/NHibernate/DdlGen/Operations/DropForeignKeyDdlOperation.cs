using System.Collections.Generic;
using System.Text;
using NHibernate.DdlGen.Model;
using NHibernate.Util;

namespace NHibernate.DdlGen.Operations
{
    public class DropForeignKeyDdlOperation : IDdlOperation
    {
        private readonly ForeignKeyModel _model;

        public DropForeignKeyDdlOperation(ForeignKeyModel model)
        {
            _model = model;
        }

        public IEnumerable<string> GetStatements(Dialect.Dialect dialect)
        {
            
            var tableName = _model.DependentTable.QuoteAndQualify(dialect);
            var name = BacktickQuoteUtil.ApplyDialectQuotes(_model.Name, dialect);
            string fkName = BacktickQuoteUtil.ApplyDialectQuotes(name, dialect);
            var sb = new StringBuilder();
            sb.AppendLine(dialect.GetIfExistsDropConstraint(fkName, tableName))
                .Append("alter table ").Append(tableName).Append(" ")
                .AppendLine(dialect.GetDropForeignKeyConstraintString(fkName))
                .AppendLine(dialect.GetIfExistsDropConstraintEnd(fkName, tableName));
            return new[] {sb.ToString()};
        }
    }

}