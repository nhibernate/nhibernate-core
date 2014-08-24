using System;
using System.Data;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a <see cref="System.String" /> to a <see cref="DbType.String" /> column.
	/// </summary>
	[Serializable]
	public class StringType : AbstractStringType
	{
		internal StringType() : base(new StringSqlType())
		{
		}

		internal StringType(StringSqlType sqlType) : base(sqlType)
		{
		}

		public override string Name
		{
			get { return "String"; }
		}
	}
}