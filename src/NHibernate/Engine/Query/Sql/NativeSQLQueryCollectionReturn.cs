using System;
using System.Collections.Generic;

namespace NHibernate.Engine.Query.Sql
{
	/// <summary> 
	/// Represents a return defined as part of a native sql query which
	/// names a collection role in the form {classname}.{collectionrole}; it
	/// is used in defining a custom sql query for loading an entity's
	/// collection in non-fetching scenarios (i.e., loading the collection
	/// itself as the "root" of the result). 
	/// </summary>
	[Serializable]
	public class NativeSQLQueryCollectionReturn : NativeSQLQueryNonScalarReturn
	{
		private readonly string ownerEntityName;
		private readonly string ownerProperty;

		/// <summary> Construct a native-sql return representing a collection initializer </summary>
		/// <param name="alias">The result alias </param>
		/// <param name="ownerEntityName">
		/// The entity-name of the entity owning the collection to be initialized. 
		/// </param>
		/// <param name="ownerProperty">
		/// The property name (on the owner) which represents
		/// the collection to be initialized.
		/// </param>
		/// <param name="propertyResults">Any user-supplied column->property mappings </param>
		/// <param name="lockMode">The lock mode to apply to the collection. </param>
		public NativeSQLQueryCollectionReturn(string alias, string ownerEntityName, string ownerProperty, IDictionary<string, string[]> propertyResults, LockMode lockMode)
			: base(alias, propertyResults, lockMode)
		{
			this.ownerEntityName = ownerEntityName;
			this.ownerProperty = ownerProperty;
		}

		/// <summary> 
		/// The class owning the collection. 
		/// </summary>
		public string OwnerEntityName
		{
			get { return ownerEntityName; }
		}

		/// <summary> 
		/// The name of the property representing the collection from the <see cref="OwnerEntityName"/>. 
		/// </summary>
		public string OwnerProperty
		{
			get { return ownerProperty; }
		}
	}
}
