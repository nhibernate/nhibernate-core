using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.DdlGen.Model;

namespace NHibernate.DdlGen.Operations
{
    public class AddTableCommentsDdlOperation : IDdlOperation
    {
        public TableCommentsModel Model { get; private set; }

        public AddTableCommentsDdlOperation(TableCommentsModel model)
        {
            Model = model;
        }

        public IEnumerable<string> GetStatements(Dialect.Dialect dialect)
        {
            if (dialect.SupportsCommentOn)
            {
                var tableName = Model.TableName.QuoteAndQualify(dialect);
                if (!String.IsNullOrEmpty(Model.Comment))
                {
                    yield return String.Format("comment on table {0} is '{1}'", tableName, Model.Comment);
                }
                foreach (var c in Model.Columns ?? Enumerable.Empty<ColumnCommentModel>())
                {
                    yield return
                        string.Format("comment on column {0}.{1} is '{2}'", tableName, c.ColumnName, c.Comment);
                }
            }
        }
    }
}