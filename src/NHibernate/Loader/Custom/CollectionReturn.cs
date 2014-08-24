namespace NHibernate.Loader.Custom
{
	/// <summary> 
	/// Represents a return which names a collection role; it
	/// is used in defining a custom query for loading an entity's
	/// collection in non-fetching scenarios (i.e., loading the collection
	/// itself as the "root" of the result). 
	/// </summary>
	public class CollectionReturn : NonScalarReturn
	{
		private readonly string ownerEntityName;
		private readonly string ownerProperty;
		private readonly ICollectionAliases collectionAliases;
		private readonly IEntityAliases elementEntityAliases;

		public CollectionReturn(string alias, string ownerEntityName, string ownerProperty,
		                        ICollectionAliases collectionAliases, IEntityAliases elementEntityAliases, LockMode lockMode)
			: base(alias, lockMode)
		{
			this.ownerEntityName = ownerEntityName;
			this.ownerProperty = ownerProperty;
			this.collectionAliases = collectionAliases;
			this.elementEntityAliases = elementEntityAliases;
		}

		/// <summary> Returns the class owning the collection. </summary>
		public string OwnerEntityName
		{
			get { return ownerEntityName; }
		}

		/// <summary> Returns the name of the property representing the collection from the <see cref="OwnerEntityName"/>. </summary>
		public string OwnerProperty
		{
			get { return ownerProperty; }
		}

		public ICollectionAliases CollectionAliases
		{
			get { return collectionAliases; }
		}

		public IEntityAliases ElementEntityAliases
		{
			get { return elementEntityAliases; }
		}
	}
}