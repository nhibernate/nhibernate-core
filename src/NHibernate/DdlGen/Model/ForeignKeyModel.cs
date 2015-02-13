
﻿using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping;
using NHibernate.Util;

namespace NHibernate.DdlGen.Model
{
    public class ForeignKeyModel
    {
        public ForeignKeyModel()
        {
            IsReferenceToPrimaryKey = true;
        }
        public string Name { get; set; }

        public DbName ReferencedTable { get; set; }
        public ICollection<string> PrimaryKeyColumns { get; set; }

        public DbName DependentTable { get; set; }
        public ICollection<string> ForeignKeyColumns { get; set; }
        public bool CascadeDelete { get; set; }

        public bool IsReferenceToPrimaryKey { get; set; }

        public string GetConstraintString(Dialect.Dialect dialect)
        {
            //string constraintName, string[] foreignKey, string referencedTable, string[] primaryKey, bool referencesPrimaryKey/
            
            var referencedTable = ReferencedTable.QuoteAndQualify(dialect);

            var foreignKeyColumns = ForeignKeyColumns.Select(c => BacktickQuoteUtil.ApplyDialectQuotes(c, dialect)).ToArray();
            var primaryKeyColumns = (PrimaryKeyColumns ?? new string[0]).Select(c => BacktickQuoteUtil.ApplyDialectQuotes(c, dialect)).ToArray();
            var fkName = BacktickQuoteUtil.ApplyDialectQuotes(Name, dialect);
            string result = dialect.GetAddForeignKeyConstraintString(fkName, foreignKeyColumns, referencedTable, primaryKeyColumns, IsReferenceToPrimaryKey);
            return CascadeDelete && dialect.SupportsCascadeDelete ? result + " on delete cascade" : result;
        }
    }

}