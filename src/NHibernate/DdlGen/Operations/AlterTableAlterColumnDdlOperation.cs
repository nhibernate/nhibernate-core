using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.DdlGen.Model;

namespace NHibernate.DdlGen.Operations
{
    public class AlterTableAlterColumnDdlOperation : IDdlOperation
    {


        public AlterTableAlterColumnDdlOperation(AddOrAlterColumnModel model)
        {
            Model = model;
        }

        public AddOrAlterColumnModel Model { get; private set; }


        public IEnumerable<string> GetStatements(Dialect.Dialect dialect)
        {
            if (!dialect.SupportsAlterColumn) return new String[0];

            var tableName = Model.Table.QuoteAndQualify(dialect);

            var sb = new StringBuilder();
            sb.Append("alter table ")
              .Append(tableName)
              .Append(" ")
              .Append(dialect.AlterColumnString).Append(" ");
            Model.Column.AppendColumnDefinitinon(sb, dialect, false);
            if (!String.IsNullOrEmpty(Model.Column.Comment))
            {
                sb.Append(dialect.GetColumnComment(Model.Column.Comment));
            }
            return new[] { sb.ToString() };
        }
    }
}