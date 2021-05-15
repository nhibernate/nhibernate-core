using System;
using NHibernate.Cfg.MappingSchema;

namespace NHibernate.Mapping.ByCode
{
	public enum SqlCheck
	{
		None,
		RowCount,
		Param,
	}

	internal static class SqlCheckExtensions
	{
		public static HbmCustomSQLCheck ToHbmSqlCheck(this SqlCheck check)
		{
			switch (check)
			{
				case SqlCheck.None:
					return HbmCustomSQLCheck.None;
				case SqlCheck.RowCount:
					return HbmCustomSQLCheck.Rowcount;
				case SqlCheck.Param:
					return HbmCustomSQLCheck.Param;
				default:
					throw new ArgumentOutOfRangeException(nameof(check), check, null);
			}
		}
	}
}
