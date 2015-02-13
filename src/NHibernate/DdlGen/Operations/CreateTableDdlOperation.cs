using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.DdlGen.Model;
using NHibernate.Util;

namespace NHibernate.DdlGen.Operations
{
    public class CreateTableDdlOperation : IDdlOperation
    {
        private readonly CreateTableModel _model;


        public CreateTableDdlOperation(CreateTableModel model)
        {
            _model = model;
            Model.UniqueIndexes = Model.UniqueIndexes ?? new IndexModel[0];
            Model.ForeignKeys = Model.ForeignKeys ?? new ForeignKeyModel[0];
            Model.Checks = Model.Checks ?? new TableCheckModel[0];
        }

        public CreateTableModel Model
        {
            get { return _model; }
        }


        public IEnumerable<string> GetStatements(Dialect.Dialect dialect)
        {

            var tableName = Model.Name.QuoteAndQualify(dialect);
            

            var sb = new StringBuilder();
            
            sb.Append(dialect.CreateTableString)
                .Append(" ")
                .Append(tableName)
                .Append(" (");
            bool needsComma = false;
            var identityColumn = Model.PrimaryKey != null && Model.PrimaryKey.Identity ? Model.PrimaryKey.Columns.First() : null;
            foreach (var c in Model.Columns)
            {
                if(needsComma)
                {
                    sb.Append(", ");
                }
                var isIdentityColumn = c == identityColumn;



                c.AppendColumnDefinitinon(sb, dialect, isIdentityColumn);
                
                needsComma = true;
            }

            if (Model.PrimaryKey != null && (dialect.GenerateTablePrimaryKeyConstraintForIdentityColumn || identityColumn == null))
            {
                sb.Append(", ");
                if (!String.IsNullOrWhiteSpace(Model.PrimaryKey.Name))
                {
                    sb.Append("constraint ").Append(BacktickQuoteUtil.ApplyDialectQuotes(Model.PrimaryKey.Name, dialect)).Append(" ");
                }
                sb.Append(dialect.PrimaryKeyString)
                   .Append(dialect.RequiresClusteredPrimaryKey? " clustered" : "")
                   .Append(" (");

                bool needsComma1 = false;
                foreach (var n in Model.PrimaryKey.Columns)
                {
                    if (needsComma1)
                    {
                        sb.Append(", ");
                    }
                    sb.Append(BacktickQuoteUtil.ApplyDialectQuotes(n.Name, dialect));
                    needsComma1 = true;
                }
                sb.Append(")");
                
            }

            var uniqueIndexs = Model.UniqueIndexes.Where(ux => ux.Unique && (!ux.IsSingleColumnUnique));

            if (!dialect.SupportsUniqueConstraintInAlterTable)
            {
                foreach (var uk in uniqueIndexs)
                {
                    sb.Append(", ");
                    if (!String.IsNullOrWhiteSpace(uk.Name))
                    {
                        sb.Append("constraint ").Append(BacktickQuoteUtil.ApplyDialectQuotes(uk.Name, dialect)).Append(" ");
                    }
                    sb.Append("unique ");
                    sb.Append(uk.GetColumnList(dialect));
                }
            }
            if (dialect.SupportsTableCheck)
            {
                foreach (var chk in Model.Checks)
                {
                    sb.Append(", ");
                    if (!String.IsNullOrWhiteSpace(chk.Name))
                    {
                        sb.Append("constraint ").Append(BacktickQuoteUtil.ApplyDialectQuotes(chk.Name, dialect)).Append(" ");
                    }
                    sb.Append("check (").Append(chk.Expression).Append(")");
                }
            }
            if (!dialect.SupportsForeignKeyConstraintInAlterTable)
            {
                foreach (var fk in Model.ForeignKeys)
                {
                    sb.Append(", ");
                    sb.Append(fk.GetConstraintString(dialect));

                }
            }
            sb.Append(")");
            sb.Append(dialect.TableTypeString);
            return new[] {sb.ToString()};
        }

    
    }
}
