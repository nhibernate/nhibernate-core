namespace NHibernate.Loader.Custom
{
	/// <summary> Spefically a fetch return that refers to a collection association. </summary>
	public class CollectionFetchReturn : FetchReturn
	{
		private readonly ICollectionAliases collectionAliases;
		private readonly IEntityAliases elementEntityAliases;

		public CollectionFetchReturn(string alias, NonScalarReturn owner, string ownerProperty,
		                             ICollectionAliases collectionAliases, IEntityAliases elementEntityAliases,
		                             LockMode lockMode) : base(owner, ownerProperty, alias, lockMode)
		{
			this.collectionAliases = collectionAliases;
			this.elementEntityAliases = elementEntityAliases;
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