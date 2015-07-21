using System.Collections.Generic;

namespace NHibernate.Hql
{
	/// <summary> 
	/// Specialized interface for filters.
	/// </summary>
	public interface IFilterTranslator : IQueryTranslator
	{
		/// <summary> 
		/// Compile a filter. This method may be called multiple
		/// times. Subsequent invocations are no-ops.
		/// </summary>
		/// <param name="collectionRole">the role name of the collection used as the basis for the filter.</param>
		/// <param name="replacements">Defined query substitutions.</param>
		/// <param name="shallow">Does this represent a shallow (scalar or entity-id) select?</param>
		void Compile(string collectionRole, IDictionary<string, string> replacements, bool shallow);
	}
}