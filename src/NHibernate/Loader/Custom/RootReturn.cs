namespace NHibernate.Loader.Custom
{
	/// <summary> 
	/// Represents a return which names a "root" entity.
	/// </summary>
	/// <remarks>
	/// A root entity means it is explicitly a "column" in the result, as opposed to
	/// a fetched association.
	/// </remarks>
	public class RootReturn : NonScalarReturn
	{
		private readonly string entityName;
		private readonly IEntityAliases entityAliases;

		public RootReturn(string alias, string entityName, IEntityAliases entityAliases, LockMode lockMode)
			: base(alias, lockMode)
		{
			this.entityName = entityName;
			this.entityAliases = entityAliases;
		}

		public string EntityName
		{
			get { return entityName; }
		}

		public IEntityAliases EntityAliases
		{
			get { return entityAliases; }
		}
	}
}