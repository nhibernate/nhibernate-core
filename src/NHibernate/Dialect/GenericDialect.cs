using System;
using System.Data;

using NHibernate.SqlTypes;

namespace NHibernate.Dialect 
{
	/// <summary>
	/// A generic SQL dialect which may or may not work on any actual databases
	/// </summary>
	public class GenericDialect : Dialect 
	{
		public GenericDialect() : base() 
		{
			Register( DbType.AnsiStringFixedLength, "CHAR($1)");
			Register( DbType.AnsiString, "VARCHAR($1)" );
			Register( DbType.Binary, "VARBINARY($1)");
			Register( DbType.Boolean, "BIT" ); 
			Register( DbType.Byte, "TINYINT" );
			Register( DbType.Currency, "MONEY");
			Register( DbType.Date, "DATE");
			Register( DbType.DateTime, "DATETIME" );
			Register( DbType.Decimal, "DECIMAL(19, $1)");
			Register( DbType.Double, "DOUBLE PRECISION" ); 
			Register( DbType.Guid, "UNIQUEIDENTIFIER" );
			Register( DbType.Int16, "SMALLINT" );
			Register( DbType.Int32, "INT" );
			Register( DbType.Int64, "BIGINT" );
			Register( DbType.Single, "REAL" ); 
			Register( DbType.StringFixedLength, "NCHAR($1)");
			Register( DbType.String, "NVARCHAR($1)" );
			Register( DbType.Time, "TIME" );
			
		}

		public override string AddColumnString 
		{
			get { return "add column"; }
		}

	}
}
