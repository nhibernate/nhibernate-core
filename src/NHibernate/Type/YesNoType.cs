using System;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a <see cref="System.Boolean" /> to a 1 char <see cref="System.Data.DbType.AnsiStringFixedLength" /> column 
	/// that stores a <code>'Y'/'N'</code> to indicate <code>true/false</code>.
	/// </summary>
	/// <remarks>
	/// If you are using schema-export to generate your tables then you need
	/// to set the column attributes: <c>length=1</c> or <c>sql-type="char(1)"</c>.
	/// 
	/// This needs to be done because in Java's JDBC there is a type for CHAR and 
	/// in ADO.NET there is not one specifically for char, so you need to tell schema
	/// export to create a char(1) column.
	/// </remarks>
	[Serializable]
	public class YesNoType : CharBooleanType
	{
		/// <summary></summary>
		public YesNoType() : base(new AnsiStringFixedLengthSqlType(1))
		{
		}

		/// <summary></summary>
		protected override sealed string TrueString
		{
			get { return "Y"; }
		}

		/// <summary></summary>
		protected override sealed string FalseString
		{
			get { return "N"; }
		}

		/// <summary></summary>
		public override string Name
		{
			get { return "YesNo"; }
		}
	}
}