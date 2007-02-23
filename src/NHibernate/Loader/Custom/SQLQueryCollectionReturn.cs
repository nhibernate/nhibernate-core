using System;
using System.Collections;

namespace NHibernate.Loader.Custom
{
	/// <summary>
	/// Represents a return defined as part of a native sql query which
	/// names a collection role in the form {classname}.{collectionrole}; it
	/// is used in defining a custom sql query for loading an entity's
	/// collection in non-fetching scenarios (i.e., loading the collection
	/// itself as the "root" of the result).
	/// </summary>
	public class SQLQueryCollectionReturn : SQLQueryNonScalarReturn
	{
		private string ownerEntityName;
		private string ownerProperty;

		public SQLQueryCollectionReturn(string alias, string ownerClass, string ownerProperty, IDictionary propertyResults,
		                                LockMode lockMode)
			: base(alias, propertyResults, lockMode)
		{
			this.ownerEntityName = ownerClass;
			this.ownerProperty = ownerProperty;
		}

		/// <summary>
		/// Returns the class owning the collection.
		/// </summary>
		public string OwnerEntityName
		{
			get { return ownerEntityName; }
		}

		/// <summary>
		/// Returns the name of the property representing the collection from the <see cref="OwnerEntityName" />.
		/// </summary>
		public string OwnerProperty
		{
			get { return ownerProperty; }
		}
	}
}