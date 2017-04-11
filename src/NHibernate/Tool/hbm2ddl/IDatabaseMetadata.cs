using NHibernate.Dialect.Schema;

namespace NHibernate.Tool.hbm2ddl
{
    public interface IDatabaseMetadata
    {
        ITableMetadata GetTableMetadata(string name, string schema, string catalog, bool isQuoted);
        bool IsSequence(object key);
        bool IsTable(object key);
    }
}
