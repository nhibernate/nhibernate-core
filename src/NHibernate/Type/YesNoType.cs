using System;

using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a System.Boolean to a single character column that stores a 
	/// "Y"/"N" to indicate true/false.
	/// </summary>
	/// <remarks>
	/// If you are using schema-export to generate your tables then you need
	/// to set the column attributes: <c>length=1</c> or <c>sql-type="char(1)"</c>.
	/// 
	/// This needs to be done because in Java's JDBC there is a type for CHAR and 
	/// in ADO.NET there is not one specifically for char, so you need to tell schema
	/// export to create a char(1) column.
	/// </remarks>
	public class YesNoType : CharBooleanType 
	{

		internal YesNoType(AnsiStringFixedLengthSqlType sqlType) : base(sqlType) 
		{
		}

		protected override sealed string TrueString 
		{
			get { return "Y"; }
		}

		protected override sealed string FalseString 
		{
			get { return "N"; }
		}

		public override string Name 
		{
			get { return "YesNo"; }
		}
	}
}