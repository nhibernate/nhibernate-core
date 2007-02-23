using System;

namespace NHibernate.Loader.Custom
{
	public class CollectionFetchReturn : FetchReturn
	{
		private readonly ICollectionAliases collectionAliases;
		private readonly IEntityAliases elementEntityAliases;

		public CollectionFetchReturn(
			String alias,
			NonScalarReturn owner,
			String ownerProperty,
			ICollectionAliases collectionAliases,
			IEntityAliases elementEntityAliases,
			LockMode lockMode)
			: base(owner, ownerProperty, alias, lockMode)
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