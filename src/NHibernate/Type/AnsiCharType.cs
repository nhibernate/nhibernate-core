using System;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a <see cref="System.Char"/> Property 
	/// to a <c>DbType.AnsiStringFixedLength</c> column.
	/// </summary>
	[Serializable]
	public class AnsiCharType : AbstractCharType
	{
		internal AnsiCharType() : base(new AnsiStringFixedLengthSqlType(1))
		{
		}

		public override string Name
		{
			get { return "AnsiChar"; }
		}
	}
}