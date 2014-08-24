using System;
using System.Collections.Generic;

namespace NHibernate.Engine.Query.Sql
{
	/// <summary> 
	/// Represents a return defined as part of a native sql query which
	/// names a fetched role. 
	/// </summary>
	[Serializable]
	public class NativeSQLQueryJoinReturn : NativeSQLQueryNonScalarReturn
	{
		private readonly string ownerAlias;
		private readonly string ownerProperty;

		/// <summary> Construct a return descriptor representing some form of fetch. </summary>
		/// <param name="alias">The result alias </param>
		/// <param name="ownerAlias">The owner's result alias </param>
		/// <param name="ownerProperty">The owner's property representing the thing to be fetched </param>
		/// <param name="propertyResults">Any user-supplied column->property mappings </param>
		/// <param name="lockMode">The lock mode to apply </param>
		public NativeSQLQueryJoinReturn(string alias, string ownerAlias, string ownerProperty, IDictionary<string, string[]> propertyResults, LockMode lockMode)
			: base(alias, propertyResults, lockMode)
		{
			this.ownerAlias = ownerAlias;
			this.ownerProperty = ownerProperty;
		}

		/// <summary> The alias of the owner of this fetched association. </summary>
		public string OwnerAlias
		{
			get { return ownerAlias; }
		}

		/// <summary> 
		/// Retrieve the property name (relative to the owner) which maps to
		/// the association to be fetched. 
		/// </summary>
		public string OwnerProperty
		{
			get { return ownerProperty; }
		}
	}
}
