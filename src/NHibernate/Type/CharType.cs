using System;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a <see cref="System.Char"/> Property 
	/// to a <c>DbType.StringFixedLength</c> column.
	/// </summary>
	public class CharType : BaseCharType
	{
		internal CharType() : base( new StringFixedLengthSqlType( 1 ) )
		{
		}

		public override string Name
		{
			get { return "Char"; }
		}
	}
}