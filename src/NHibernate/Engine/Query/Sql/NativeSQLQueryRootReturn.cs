using System;
using System.Collections.Generic;

namespace NHibernate.Engine.Query.Sql
{
	/// <summary> 
	/// Represents a return defined as part of a native sql query which
	/// names a "root" entity.  A root entity means it is explicitly a
	/// "column" in the result, as opposed to a fetched relationship or role. 
	/// </summary>
	[Serializable]
	public class NativeSQLQueryRootReturn : NativeSQLQueryNonScalarReturn
	{
		private readonly string returnEntityName;

		/// <summary> 
		/// Construct a return representing an entity returned at the root
		/// of the result.
		///  </summary>
		/// <param name="alias">The result alias </param>
		/// <param name="entityName">The entity name. </param>
		/// <param name="lockMode">The lock mode to apply </param>
		public NativeSQLQueryRootReturn(string alias, string entityName, LockMode lockMode)
			: this(alias, entityName, null, lockMode)
		{
		}

		/// <summary> 
		/// Construct a return representing an entity returned at the root
		/// of the result. 
		/// </summary>
		/// <param name="alias">The result alias </param>
		/// <param name="entityName">The entity name. </param>
		/// <param name="propertyResults">Any user-supplied column->property mappings </param>
		/// <param name="lockMode">The lock mode to apply </param>
		public NativeSQLQueryRootReturn(string alias, string entityName, IDictionary<string,string[]> propertyResults, LockMode lockMode)
			: base(alias, propertyResults, lockMode)
		{
			returnEntityName = entityName;
		}

		/// <summary> The name of the entity to be returned. </summary>
		public string ReturnEntityName
		{
			get { return returnEntityName; }
		}
	}
}
