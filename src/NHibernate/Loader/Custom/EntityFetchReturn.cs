using System;

namespace NHibernate.Loader.Custom
{
	public class EntityFetchReturn : FetchReturn
	{
		private readonly IEntityAliases entityAliases;

		public EntityFetchReturn(
			String alias,
			IEntityAliases entityAliases,
			NonScalarReturn owner,
			String ownerProperty,
			LockMode lockMode)
			: base(owner, ownerProperty, alias, lockMode)
		{
			this.entityAliases = entityAliases;
		}

		public IEntityAliases EntityAliases
		{
			get { return entityAliases; }
		}
	}
}