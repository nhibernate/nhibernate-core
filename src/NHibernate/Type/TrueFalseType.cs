using System;

using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a <see cref="System.Boolean" /> to a 1 char <see cref="DbType.AnsiStringFixedLength" /> column 
	/// that stores a <code>'T'/'F'</code> to indicate <code>true/false</code>.
	/// </summary>
	/// <remarks>
	/// If you are using schema-export to generate your tables then you need
	/// to set the column attributes: <c>length=1</c> or <c>sql-type="char(1)"</c>.
	/// 
	/// This needs to be done because in Java's JDBC there is a type for CHAR and 
	/// in ADO.NET there is not one specifically for char, so you need to tell schema
	/// export to create a char(1) column.
	/// </remarks>
	public class TrueFalseType: CharBooleanType 
	{
		internal TrueFalseType() : base( new AnsiStringFixedLengthSqlType(1) ) 
		{
		}

		protected override sealed string TrueString {
			get { return "T"; }
		}

		protected override sealed string FalseString {
			get { return "F"; }
		}

		public override string Name {
			get { return "TrueFalse"; }
		}
	}
}