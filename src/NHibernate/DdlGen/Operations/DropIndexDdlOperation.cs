using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.DdlGen.Model;
using NHibernate.Util;

namespace NHibernate.DdlGen.Operations
{
    public class DropIndexDdlOperation : IDdlOperation
    {
        private readonly IndexModel _model;
        private readonly bool _includeIfExists;


        public DropIndexDdlOperation(IndexModel model, bool includeIfExists)
        {
            _model = model;
            _includeIfExists = includeIfExists;
        }

        public bool IncludeIfExists
        {
            get { return _includeIfExists; }
        }

        public IndexModel Model
        {
            get { return _model; }
        }

        public IEnumerable<string> GetStatements(Dialect.Dialect dialect)
        {
            
            string tableName = Model.TableName.QuoteAndQualify(dialect);
            string indexName = BacktickQuoteUtil.ApplyDialectQuotes(Model.Name, dialect);
            var sb = new StringBuilder();
            if (IncludeIfExists)
            {
                sb.Append(dialect.GetIfExistsDropConstraint(indexName, tableName)).Append(" ");
                sb.AppendLine();
            }
            if (Model.Unique && dialect.SupportsUniqueConstraintInAlterTable)
            {

                sb.AppendFormat("alter table {0} drop constraint {1}", tableName, indexName);
            }
            else
            {
                //This reflects the current behavior of the ddl gen.  Should this be the behavior though?
                sb.AppendFormat("drop index {0}", StringHelper.Qualify(tableName, indexName));
            }
            if (IncludeIfExists)
            {
                sb.AppendLine();
                sb.Append(dialect.GetIfExistsDropConstraintEnd(indexName, tableName));
            }
            return new[] {sb.ToString()};

        }

        
    }
}