using System;

namespace NHibernate.Loader.Custom
{
	public class RootReturn : NonScalarReturn
	{
		private readonly string entityName;
		private readonly IEntityAliases entityAliases;

		public RootReturn(
			String alias,
			String entityName,
			IEntityAliases entityAliases,
			LockMode lockMode)
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