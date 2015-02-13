using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.DdlGen.Model;
using NHibernate.Mapping;
using NHibernate.Util;

namespace NHibernate.DdlGen.Operations
{
    public class CreateIndexDdlOperation : IDdlOperation
    {
        private readonly IndexModel _model;

        public CreateIndexDdlOperation(IndexModel model)
        {
            _model = model;
        }

        public IndexModel Model
        {
            get { return _model; }
        }


        public IEnumerable<string> GetStatements(Dialect.Dialect dialect)
        {
            var tableName = Model.TableName.QuoteAndQualify(dialect);
            var indexName = BacktickQuoteUtil.ApplyDialectQuotes(Model.Name, dialect);
            if (Model.Unique && dialect.SupportsUniqueConstraintInAlterTable)
            {
                var b1 = new StringBuilder()
                    .Append("alter table ")
                    .Append(tableName)
                    .Append(dialect.GetAddUniqueConstraintString(indexName))
                    .Append(Model.GetColumnList(dialect));

                return new[] {b1.ToString()};
            }

            var b2 = new StringBuilder("create")
                .Append(Model.Unique ? " unique" : "")
                .Append(Model.Clustered? " clustered": "")
                .Append(" index ")
                .Append(indexName)
                .Append(" on ")
                .Append(tableName).Append(" ")
                .Append(Model.GetColumnList(dialect));
            return new[] {b2.ToString()};

        }
    }
}
