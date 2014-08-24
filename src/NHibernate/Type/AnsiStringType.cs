using System;
using System.Data;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a <see cref="System.String"/> Property 
	/// to a <see cref="DbType.AnsiString"/> column.
	/// </summary>
	[Serializable]
	public class AnsiStringType : AbstractStringType
	{
		internal AnsiStringType() : base(new AnsiStringSqlType()) { }

		internal AnsiStringType(AnsiStringSqlType sqlType)
			: base(sqlType)
		{
		}

		/// <summary></summary>
		public override string Name
		{
			get { return "AnsiString"; }
		}
	}
}