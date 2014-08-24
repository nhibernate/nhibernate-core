using NHibernate.Engine;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Loader.Entity
{
	public class CascadeEntityJoinWalker : AbstractEntityJoinWalker
	{
		private readonly CascadingAction cascadeAction;

		public CascadeEntityJoinWalker(IOuterJoinLoadable persister, CascadingAction action,
		                               ISessionFactoryImplementor factory)
			: base(persister, factory, new CollectionHelper.EmptyMapClass<string, IFilter>())
		{
			cascadeAction = action;
			SqlStringBuilder whereCondition = WhereString(Alias, persister.IdentifierColumnNames, 1)
				//include the discriminator and class-level where, but not filters
				.Add(persister.FilterFragment(Alias, new CollectionHelper.EmptyMapClass<string, IFilter>()));

			InitAll(whereCondition.ToSqlString(), SqlString.Empty, LockMode.Read);
		}

		protected override bool IsJoinedFetchEnabled(IAssociationType type, FetchMode config, CascadeStyle cascadeStyle)
		{
			return
				(type.IsEntityType || type.IsCollectionType) && (cascadeStyle == null || cascadeStyle.DoCascade(cascadeAction));
		}

		protected override bool IsTooManyCollections
		{
			get { return CountCollectionPersisters(associations) > 0; }
		}

		public override string Comment
		{
			get { return "load " + Persister.EntityName; }
		}
	}
}
