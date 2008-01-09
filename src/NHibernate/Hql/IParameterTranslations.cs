using System.Collections.Generic;
using NHibernate.Type;

namespace NHibernate.Hql
{
	public interface IParameterTranslations
	{
		bool SupportsOrdinalParameterMetadata { get; }

		int OrdinalParameterCount { get; }

		int GetOrdinalParameterSqlLocation(int ordinalPosition);

		IType GetOrdinalParameterExpectedType(int ordinalPosition);

		IEnumerable<string> GetNamedParameterNames();

		int[] GetNamedParameterSqlLocations(string name);

		IType GetNamedParameterExpectedType(string name);
	}
}