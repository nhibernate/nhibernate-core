using System.Collections.Generic;
using NHibernate.Param;
using NHibernate.SqlCommand;

namespace NHibernate.Loader.Custom
{
	/// <summary> 
	/// Extension point allowing any SQL query with named and positional parameters
	/// to be executed by Hibernate, returning managed entities, collections and
	/// simple scalar values. 
	/// </summary>
	public interface ICustomQuery
	{
		/// <summary> The SQL query string to be performed. </summary>
		SqlString SQL { get; }

		/// <summary> 
		/// Any query spaces to apply to the query execution.  Query spaces are
		/// used in Hibernate's auto-flushing mechanism to determine which
		/// entities need to be checked for pending changes. 
		/// </summary>
		ISet<string> QuerySpaces { get; }

		/// <summary> 
		/// A collection of <see cref="IReturn"/> descriptors describing the
		/// ADO result set to be expected and how to map this result set. 
		/// </summary>
		IList<IReturn> CustomQueryReturns { get; }

		IEnumerable<IParameterSpecification> CollectedParametersSpecifications { get; }
	}
}