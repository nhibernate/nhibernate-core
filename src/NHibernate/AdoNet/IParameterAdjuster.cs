using System.Data.Common;
using NHibernate.SqlTypes;
using NHibernate.Type;

namespace NHibernate.AdoNet
{
	internal interface IParameterAdjuster
	{
		void AdjustParameterForValue(DbParameter parameter, SqlType sqlType, object value);
	}
}
