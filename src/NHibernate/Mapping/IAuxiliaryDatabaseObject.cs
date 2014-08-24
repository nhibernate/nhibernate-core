using System.Collections.Generic;

namespace NHibernate.Mapping
{
	/// <summary> 
	/// Auxiliary database objects (i.e., triggers, stored procedures, etc) defined
	/// in the mappings.  Allows Hibernate to manage their lifecycle as part of
	/// creating/dropping the schema. 
	/// </summary>
	public interface IAuxiliaryDatabaseObject : IRelationalModel
	{
		/// <summary> 
		/// Add the given dialect name to the scope of dialects to which
		/// this database object applies. 
		/// </summary>
		/// <param name="dialectName">The name of a dialect. </param>
		void AddDialectScope(string dialectName);

		/// <summary> 
		/// Does this database object apply to the given dialect? 
		/// </summary>
		/// <param name="dialect">The dialect to check against. </param>
		/// <returns> True if this database object does apply to the given dialect. </returns>
		bool AppliesToDialect(Dialect.Dialect dialect);

		/// <summary>
		/// Gets called by NHibernate to pass the configured type parameters to the implementation.
		/// </summary>
		void SetParameterValues(IDictionary<string, string> parameters);
	}
}