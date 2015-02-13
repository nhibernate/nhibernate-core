using System;
using System.Linq;
using System.Text.RegularExpressions;
using NHibernate.Util;

namespace NHibernate.DdlGen.Model
{
    public class DbName : IEquatable<DbName>
    {


        public DbName(string catalog, string schema, string name)
        {
            Catalog = catalog;
            Schema = schema;
            Name = name;

        }
        public DbName(string name)
        {
            var parts = Regex.Matches(name, "(`[^`]+`|#?[A-z0-9_]*)")
                 .Cast<Match>()
                 .Where(x => x.Length > 0)
                 .Select(x => x.Value)
                 .ToArray();
            if (parts.Length == 0 || parts.Length > 3)
            {
                throw new ArgumentException("Name must be a db name in 1-3 parts, optionally quoted with `", "name");
            }
            if (parts.Length == 1)
            {
                Catalog = null;
                Schema = null;
                Name = parts[0];
            }
            if (parts.Length == 2)
            {
                Catalog = null;
                Schema = parts[0];
                Name = parts[1];
            }
            if (parts.Length == 3)
            {
                Catalog = parts[0];
                Schema = parts[1];
                Name = parts[2];
            }
        }
        public string Name { get; protected set; }
        public string Schema { get; protected set; }
        public string Catalog { get; protected set; }


        public string QuoteAndQualify(Dialect.Dialect dialect)
        {

            var quotedName = BacktickQuoteUtil.ApplyDialectQuotes(Name, dialect);
            var quotedSchema = BacktickQuoteUtil.ApplyDialectQuotes(Schema, dialect);
            var quotedCatalog = BacktickQuoteUtil.ApplyDialectQuotes(Catalog, dialect);
            var result = dialect.Qualify(quotedCatalog, quotedSchema, quotedName);
            return result;
        }

        public bool Equals(DbName that)
        {
            if (that == null) return false;
            return that.Catalog == this.Catalog && that.Schema == this.Schema && that.Name == this.Name;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as DbName);
        }

        public override string ToString()
        {
            if (Catalog != null)
            {
                return Catalog + "." + Schema + "." + Name;
            }
            if (Schema != null)
            {
                return Schema + "." + Name;
            }
            return Name;
        }
    }
}
