using System.Collections.Generic;
using NHibernate.DdlGen.Model;
using NHibernate.Util;

namespace NHibernate.DdlGen.Operations
{
    public class AlterTableRenameColumnOperation : IDdlOperation
    {
        public RenameColumnModel Model { get; private set; }

        public AlterTableRenameColumnOperation(RenameColumnModel model)
        {
            Model = model;
        }

        public IEnumerable<string> GetStatements(Dialect.Dialect dialect)
        {
            var tableName = Model.Table.QuoteAndQualify(dialect);
            var oldColumnName = BacktickQuoteUtil.ApplyDialectQuotes(Model.OldColumnName, dialect);
            var newColumnName = BacktickQuoteUtil.ApplyDialectQuotes(Model.NewColumnName, dialect);

            yield return dialect.GetRenameColumnString(tableName, oldColumnName, newColumnName);
        }
    }
}
