using System.Collections.Generic;

namespace NHibernate.Mapping
{
	public interface IAuxiliaryDatabaseObject : IRelationalModel
	{
		void AddDialectScope(string dialectName);
		bool AppliesToDialect(Dialect.Dialect dialect);
		/// <summary>
		/// Gets called by NHibernate to pass the configured type parameters to the implementation.
		/// </summary>
		void SetParameterValues(IDictionary<string, string> parameters);
	}
}