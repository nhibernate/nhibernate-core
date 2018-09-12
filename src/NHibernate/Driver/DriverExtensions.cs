using System.Data.Common;
using NHibernate.AdoNet;
using NHibernate.SqlTypes;
using NHibernate.Type;

namespace NHibernate.Driver
{
	internal static class DriverExtensions
	{
		internal static void AdjustParameterForValue(this IDriver driver, DbParameter parameter, SqlType sqlType, object value)
		{
			var adjustingDriver = driver as IParameterAdjuster;
			adjustingDriver?.AdjustParameterForValue(parameter, sqlType, value);
		}
	}
}
