using System;

namespace NHibernate.Mapping
{
	public interface IAuxiliaryDatabaseObject : IRelationalModel
	{
		void AddDialectScope(string dialectName);
		bool AppliesToDialect(Dialect.Dialect dialect);
	}
}