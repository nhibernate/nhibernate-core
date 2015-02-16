using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using NHibernate.DdlGen.Model;
using NHibernate.Util;

namespace NHibernate.DdlGen.Operations
{
    public class DropForeignKeyDdlOperation : IDdlOperation
    {
        public ForeignKeyModel Model { get; private set; }

        public DropForeignKeyDdlOperation(ForeignKeyModel model)
        {
            Model = model;
        }

        public IEnumerable<string> GetStatements(Dialect.Dialect dialect)
        {
            
            var tableName = Model.DependentTable.QuoteAndQualify(dialect);
            var name = BacktickQuoteUtil.ApplyDialectQuotes(Model.Name, dialect);
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