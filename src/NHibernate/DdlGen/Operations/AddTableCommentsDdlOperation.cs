using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.DdlGen.Model;

namespace NHibernate.DdlGen.Operations
{
    public class AddTableCommentsDdlOperation : IDdlOperation
    {
        private readonly TableCommentsModel _model;

        public AddTableCommentsDdlOperation(TableCommentsModel model)
        {
            _model = model;
        }

        public IEnumerable<string> GetStatements(Dialect.Dialect dialect)
        {
            if (dialect.SupportsCommentOn)
            {
                var tableName = _model.TableName.QuoteAndQualify(dialect);
                if (!String.IsNullOrEmpty(_model.Comment))
                {
                    yield return String.Format("comment on table {0} is '{1}'", tableName, _model.Comment);
                }
                foreach (var c in _model.Columns ?? Enumerable.Empty<ColumnCommentModel>())
                {
                    yield return
                        string.Format("comment on column {0}.{1} is '{2}'", tableName, c.ColumnName, c.Comment);
                }
            }
        }
    }
}