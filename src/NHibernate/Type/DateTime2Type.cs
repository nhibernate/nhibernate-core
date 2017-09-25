using System;
using System.Data;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	// Obsolete since v5.0
	/// <summary>
	/// Maps a <see cref="System.DateTime" /> Property to a <see cref="DbType.DateTime2"/>
	/// </summary>
	[Obsolete("Use DateTimeType instead, it uses DateTime2 with dialects supporting it.")]
	[Serializable]
	public class DateTime2Type : AbstractDateTimeType
	{
		internal DateTime2Type() : base(SqlTypeFactory.DateTime2)
		{
		}

		/// <inheritdoc />
		public override string Name => "DateTime2";
	}
}
