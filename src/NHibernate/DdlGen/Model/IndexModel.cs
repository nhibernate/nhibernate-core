using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using NHibernate.Util;

namespace NHibernate.DdlGen.Model
{
    public class IndexModel
    {
        public string Name { get; set; }
        public ICollection<string> Columns { get; set; }
        public bool Unique { get; set; }

        public bool Clustered { get; set; }



       

        public bool IsSingleColumnUnique
        {
            get { return Columns.Count == 1 && Unique; }
        }

        public DbName TableName { get; set; }

        public string GetColumnList(Dialect.Dialect dialect)
        {
            
            var sb = new StringBuilder().Append("(");
            var needsComma = false;
            foreach (var c in Columns)
            {
                if (needsComma)
                {
                    sb.Append(", ");
                }

                needsComma = true;
                sb.Append(BacktickQuoteUtil.ApplyDialectQuotes(c, dialect));
            }
            sb.Append(")");
            return sb.ToString();

        }
    }
}