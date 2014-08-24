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
	[Serializable]
	public class StringClobType : StringType
	{
		/// <summary></summary>
		internal StringClobType() : base(new StringClobSqlType())
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sqlType"></param>
		internal StringClobType(StringSqlType sqlType) : base(sqlType)
		{
		}

		/// <summary></summary>
		public override string Name
		{
			get { return "StringClob"; }
		}
	}
}