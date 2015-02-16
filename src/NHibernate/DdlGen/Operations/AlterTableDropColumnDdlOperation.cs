using System;
using System.Collections.Generic;
using NHibernate.DdlGen.Model;
using NHibernate.Util;

namespace NHibernate.DdlGen.Operations
{
    public class AlterTableDropColumnDdlOperation : IDdlOperation
    {

        public DropColumnModel Model { get; private set; }

        public AlterTableDropColumnDdlOperation(DropColumnModel model)
        {
            Model = model;

        }

        public IEnumerable<string> GetStatements(Dialect.Dialect dialect)
        {
            if (dialect.SupportsDropColumn)
            {

                var tableName = Model.Table.QuoteAndQualify(dialect);
                var columnName = BacktickQuoteUtil.ApplyDialectQuotes(Model.Column, dialect);

                return new[] {String.Format("alter table {0} drop column {1}", tableName, columnName)};

            }
            return new string[0];
        }
    }
}
