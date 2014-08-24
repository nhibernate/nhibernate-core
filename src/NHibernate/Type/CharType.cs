using System;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a <see cref="System.Char"/> Property 
	/// to a <c>DbType.StringFixedLength</c> column.
	/// </summary>
	[Serializable]
	public class CharType : AbstractCharType
	{
		internal CharType() : base(new StringFixedLengthSqlType(1))
		{
		}

		public override string Name
		{
			get { return "Char"; }
		}
	}
}