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
		/// <summary>
		/// Default constructor.
		/// </summary>
		public DateTime2Type() : base(SqlTypeFactory.DateTime2)
		{
		}

		/// <summary>
		/// Constructor for specifying a datetime with a scale. Use <see cref="SqlTypeFactory.GetDateTime2"/>.
		/// </summary>
		/// <param name="sqlType">The sql type to use for the type.</param>
		public DateTime2Type(DateTime2SqlType sqlType) : base(sqlType)
		{
		}

		/// <inheritdoc />
		public override string Name => "DateTime2";
	}
}
