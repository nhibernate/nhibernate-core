using System;
using System.Data;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps the Year, Month, and Day of a <see cref="System.DateTime"/> Property to a 
	/// <see cref="DbType.Date"/> column. Specify <see cref="DateTimeKind.Local"/> when reading
	/// dates from <see cref="System.Data.Common.DbDataReader"/>.
	/// </summary>
	[Serializable]
	public class LocalDateType : DateType
	{
		/// <inheritdoc />
		protected override DateTimeKind Kind => DateTimeKind.Local;
	}
}
