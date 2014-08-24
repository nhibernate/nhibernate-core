using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Loader.Entity
{
	/// <summary>
	/// A walker for loaders that fetch entities
	/// </summary>
	/// <seealso cref="EntityLoader"/>
	public class EntityJoinWalker : AbstractEntityJoinWalker
	{
		private readonly LockMode lockMode;

		public EntityJoinWalker(IOuterJoinLoadable persister, string[] uniqueKey, int batchSize, LockMode lockMode,
		                        ISessionFactoryImplementor factory, IDictionary<string, IFilter> enabledFilters)
			: base(persister, factory, enabledFilters)
		{
			this.lockMode = lockMode;

			SqlStringBuilder whereCondition = WhereString(Alias, uniqueKey, batchSize)
				//include the discriminator and class-level where, but not filters
				.Add(persister.FilterFragment(Alias, new CollectionHelper.EmptyMapClass<string, IFilter>()));

			InitAll(whereCondition.ToSqlString(), SqlString.Empty, lockMode);
		}

		/// <summary>
		/// Override to use the persister to change the table-alias for columns in join-tables
		/// </summary>
		protected override string GenerateAliasForColumn(string rootAlias, string column)
		{
			return Persister.GenerateTableAliasForColumn(rootAlias, column);
		}

		/// <summary>
		/// Disable outer join fetching if this loader obtains an
		/// upgrade lock mode
		/// </summary>
		protected override bool IsJoinedFetchEnabled(IAssociationType type, FetchMode config, CascadeStyle cascadeStyle)
		{
			return lockMode.GreaterThan(LockMode.Read) ? false : base.IsJoinedFetchEnabled(type, config, cascadeStyle);
		}

		public override string Comment
		{
			get { return "load " + Persister.EntityName; }
		}
	}
}
