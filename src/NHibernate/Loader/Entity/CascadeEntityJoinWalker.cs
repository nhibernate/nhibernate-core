using NHibernate.Engine;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Loader.Entity
{
	public class CascadeEntityJoinWalker : AbstractEntityJoinWalker
	{
		private Cascades.CascadingAction cascadeAction;

		public CascadeEntityJoinWalker(IOuterJoinLoadable persister, Cascades.CascadingAction action,
		                               ISessionFactoryImplementor factory)
			: base(persister, factory, new CollectionHelper.EmptyMapClass<string, IFilter>())
		{
			cascadeAction = action;
			SqlStringBuilder whereCondition = WhereString(Alias, persister.IdentifierColumnNames, persister.IdentifierType, 1)
				//include the discriminator and class-level where, but not filters
				.Add(persister.FilterFragment(Alias, new CollectionHelper.EmptyMapClass<string, IFilter>()));

			InitAll(whereCondition.ToSqlString(), "", LockMode.Read);
		}

		protected override bool IsJoinedFetchEnabled(IAssociationType type, FetchMode config,
		                                             Cascades.CascadeStyle cascadeStyle)
		{
			return (type.IsEntityType || type.IsCollectionType) &&
			       (cascadeStyle == null || cascadeStyle.DoCascade(cascadeAction));
		}

		protected override bool IsTooManyCollections()
		{
			return CountCollectionPersisters(associations) > 1;
		}

		public override string Comment
		{
			get { return "load " + Persister.ClassName; }
		}
	}
}