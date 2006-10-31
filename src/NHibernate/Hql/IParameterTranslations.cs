using System;
using NHibernate.Type;
using System.Collections;

namespace NHibernate.Hql
{
	public interface IParameterTranslations
	{
		bool SupportsOrdinalParameterMetadata { get;}

		int OrdinalParameterCount { get;}

		int GetOrdinalParameterSqlLocation(int ordinalPosition);

		IType GetOrdinalParameterExpectedType(int ordinalPosition);

		IList GetNamedParameterNames();

		int[] GetNamedParameterSqlLocations(string name);

		IType GetNamedParameterExpectedType(string name);
	}
}
