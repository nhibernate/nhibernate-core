using System;
using System.Collections.Generic;
using NHibernate.DdlGen.Model;
using NHibernate.Util;

namespace NHibernate.DdlGen.Operations
{
    public class AlterTableDropColumnDdlOperation : IDdlOperation
    {
        private readonly DropColumnModel _model;

        public AlterTableDropColumnDdlOperation(DropColumnModel model)
        {
            _model = model;
        }

        public IEnumerable<string> GetStatements(Dialect.Dialect dialect)
        {
            if (dialect.SupportsDropColumn)
            {
                var tableName = _model.Table.QuoteAndQualify(dialect);
                var columnName = BacktickQuoteUtil.ApplyDialectQuotes(_model.Column, dialect);
                return new[] {String.Format("alter table {0} drop column {1}", tableName, columnName)};

            }
            return new string[0];
        }
    }
}
