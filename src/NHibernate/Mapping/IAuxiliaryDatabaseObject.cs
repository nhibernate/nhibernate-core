using System.Collections.Generic;
using NHibernate.DdlGen.Operations;
using NHibernate.Engine;

namespace NHibernate.Mapping
{
	/// <summary> 
	/// Auxiliary database objects (i.e., triggers, stored procedures, etc) defined
	/// in the mappings.  Allows Hibernate to manage their lifecycle as part of
	/// creating/dropping the schema. 
	/// </summary>
	public interface IAuxiliaryDatabaseObject 
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

        /// <summary>
        /// Generates the operation to create this db object
        /// </summary>
        /// <param name="dialect"></param>
        /// <param name="mapping"></param>
        /// <param name="defaultCatalog"></param>
        /// <param name="defaultSchema"></param>
        /// <returns></returns>
        IDdlOperation GetCreateOperation(Dialect.Dialect dialect, IMapping mapping, string defaultCatalog, string defaultSchema);

        /// <summary>
        /// Generates the operation to drop this db object
        /// </summary>
        /// <param name="dialect"></param>
        /// <param name="mapping"></param>
        /// <param name="defaultCatalog"></param>
        /// <param name="defaultSchema"></param>
        /// <returns></returns>
        IDdlOperation GetDropOperation(Dialect.Dialect dialect, IMapping mapping, string defaultCatalog, string defaultSchema);
	}
}