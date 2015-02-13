using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Description;
using System.Text;
using NHibernate.DdlGen.Model;
using NHibernate.Engine;
using NHibernate.Util;

namespace NHibernate.DdlGen.Operations
{
    public class CreateForeignKeyOperation : IDdlOperation
    {
        private readonly ForeignKeyModel _model;

        public CreateForeignKeyOperation(ForeignKeyModel model)
        {
            _model = model;
        }

        public ForeignKeyModel Model
        {
            get { return _model; }
        }

        public IEnumerable<string> GetStatements(Dialect.Dialect dialect)
        {
            if (!dialect.SupportsForeignKeyConstraintInAlterTable)
            {
                return Enumerable.Empty<string>();
            }
            var referencedTable = _model.ReferencedTable.QuoteAndQualify(dialect);
            var dependentTable = _model.DependentTable.QuoteAndQualify(dialect);
            var foreignKeyColumns =
                _model.ForeignKeyColumns.Select(c => BacktickQuoteUtil.ApplyDialectQuotes(c, dialect)).ToArray();
            var primaryKeyColumns =
                _model.PrimaryKeyColumns.Select(c => BacktickQuoteUtil.ApplyDialectQuotes(c, dialect)).ToArray();
            var fksql = dialect.GetAddForeignKeyConstraintString(_model.Name, foreignKeyColumns, referencedTable,
                                                                 primaryKeyColumns, _model.IsReferenceToPrimaryKey);
            var sb = new StringBuilder();

            sb.AppendFormat("alter table {0} {1}", dependentTable, fksql);
            if (_model.CascadeDelete)
            {
                sb.Append(" on delete cascade ");
            }


            return new[] {sb.ToString()};
        }
    }
}