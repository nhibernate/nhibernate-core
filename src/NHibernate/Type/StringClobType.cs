using System;

using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	///	Maps a <see cref="System.String" /> Property to an 
	///	<see cref="System.String" /> column that can store a CLOB.
	/// </summary>
	/// <remarks>
	/// This is only needed by DataProviders (SqlClient) that need to specify a Size for the
	/// IDbDataParameter.  Most DataProvider(Oralce) don't need to set the Size so a StringType
	/// would work just fine.
	/// </remarks>
	public class StringClobType : StringType
	{
		internal StringClobType() : base( new StringClobSqlType() ) 
		{
		}

		internal StringClobType(StringSqlType sqlType) : base(sqlType) 
		{
		}

		public override string Name
		{
			get	{return "StringClob"; }
		}
	}
}
