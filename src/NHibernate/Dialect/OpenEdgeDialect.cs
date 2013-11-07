using System.Data;
using NHibernate.Cfg;
using NHibernate.Dialect.Function;

namespace NHibernate.Dialect
{
    /// <summary>
    /// This dialect applies to Progress OpenEdge V 10.2A 
    /// using an ODBC driver from Progress DataDirect.
    /// </summary>
    public class OpenEdgeDialect : Dialect
    {
        public OpenEdgeDialect()
        {
            RegisterColumnType(DbType.Boolean, "bit");
            RegisterColumnType(DbType.Int64, "INT64");
            RegisterColumnType(DbType.Int16, "smallint");
            RegisterColumnType(DbType.Int32, "integer");
            RegisterColumnType(DbType.StringFixedLength, "character(255)");  //should this be character($1)?
            RegisterColumnType(DbType.String, "varchar(255)");
            RegisterColumnType(DbType.Single, "real");
            RegisterColumnType(DbType.Double, "double precision");
            RegisterColumnType(DbType.Date, "date");
            RegisterColumnType(DbType.Time, "time");
            RegisterColumnType(DbType.DateTime, "timestamp");
            RegisterColumnType(DbType.Binary, "varbinary($l)");
            RegisterFunction("concat", new VarArgsSQLFunction(NHibernateUtil.String, "concat(", "+", ",'')"));            

            DefaultProperties[Environment.ConnectionDriver] = "NHibernate.Driver.OdbcDriver";
            DefaultProperties[Environment.PrepareSql] = "true";

            //Other possible types that have not been implemented:
            //RegisterColumnType(DbType., "tinyint");
            //RegisterColumnType(DbType.NUMERIC, "numeric($p,$s)");
            //RegisterColumnType(DbType.AnsiStringFixedLength, "CHAR($l)");
            //RegisterColumnType(DbType.AnsiString, 255, "VARCHAR($l)");
            //RegisterColumnType(DbType.AnsiString, 32739, "LVARCHAR($l)");
            //RegisterColumnType(DbType.AnsiString, 2147483647, "TEXT");
            //RegisterColumnType(DbType.AnsiString, "VARCHAR(255)");
            //RegisterColumnType(DbType.Binary, 2147483647, "BYTE");
            //RegisterColumnType(DbType.Binary, "BYTE");
            //RegisterColumnType(DbType.Boolean, "BOOLEAN");
            //RegisterColumnType(DbType.Currency, "DECIMAL(16,4)");
            //RegisterColumnType(DbType.Byte, "SMALLINT");
            //RegisterColumnType(DbType.Date, "DATE");
            //RegisterColumnType(DbType.DateTime, "datetime year to fraction(5)");
            //RegisterColumnType(DbType.Decimal, "DECIMAL(19, 5)");
            //RegisterColumnType(DbType.Decimal, 19, "DECIMAL($p, $s)");
            //RegisterColumnType(DbType.Double, "DOUBLE");
            //RegisterColumnType(DbType.Int16, "SMALLINT");
            //RegisterColumnType(DbType.Int32, "INTEGER");
            //RegisterColumnType(DbType.Int64, "BIGINT");
            //RegisterColumnType(DbType.Single, "SmallFloat");
            //RegisterColumnType(DbType.Time, "datetime hour to second");
            //RegisterColumnType(DbType.StringFixedLength, "CHAR($l)");
            //RegisterColumnType(DbType.String, 255, "VARCHAR($l)");
            //RegisterColumnType(DbType.String, 32739, "LVARCHAR($l)");
            //RegisterColumnType(DbType.String, 2147483647, "TEXT");
            //RegisterColumnType(DbType.String, "VARCHAR(255)");
        }
        
        public bool HasAlterTable
        {
            get { return true;}
        }

        public override string AddColumnString
        {
            get
            {
                return "add column";
            }
        }

        public override bool QualifyIndexName
        {
            get
            {
                return false;
            }
        }

    }
}
