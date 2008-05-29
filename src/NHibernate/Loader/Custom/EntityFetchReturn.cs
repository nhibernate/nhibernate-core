namespace NHibernate.Loader.Custom
{
	/// <summary> Spefically a fetch return that refers to an entity association. </summary>
	public class EntityFetchReturn : FetchReturn
	{
		private readonly IEntityAliases entityAliases;

		public EntityFetchReturn(string alias, IEntityAliases entityAliases, NonScalarReturn owner, string ownerProperty,
		                         LockMode lockMode) : base(owner, ownerProperty, alias, lockMode)
		{
			this.entityAliases = entityAliases;
		}

		public IEntityAliases EntityAliases
		{
			get { return entityAliases; }
		}
	}
}