using System;

namespace NHibernate.Loader.Custom
{
	public class CollectionReturn : NonScalarReturn
	{
		private readonly string ownerEntityName;
		private readonly string ownerProperty;
		private readonly ICollectionAliases collectionAliases;
		private readonly IEntityAliases elementEntityAliases;

		public CollectionReturn(
			String alias,
			String ownerEntityName,
			String ownerProperty,
			ICollectionAliases collectionAliases,
			IEntityAliases elementEntityAliases,
			LockMode lockMode)
			: base(alias, lockMode)
		{
			this.ownerEntityName = ownerEntityName;
			this.ownerProperty = ownerProperty;
			this.collectionAliases = collectionAliases;
			this.elementEntityAliases = elementEntityAliases;
		}

		public string OwnerEntityName
		{
			get { return ownerEntityName; }
		}

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