using System;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	class EntityJoinJoinSequenceImpl : JoinSequence
	{
		private readonly EntityType _entityType;
		private readonly string _tableName;
		private readonly string _tableAlias;
		private readonly JoinType _joinType;

		public EntityJoinJoinSequenceImpl(ISessionFactoryImplementor factory, EntityType entityType, string tableName, string tableAlias, JoinType joinType):base(factory)
		{
			_entityType = entityType;
			_tableName = tableName;
			_tableAlias = tableAlias;
			_joinType = joinType;
		}

		internal override JoinFragment ToJoinFragment(
			IDictionary<string, IFilter> enabledFilters,
			bool includeExtraJoins,
			SqlString withClauseFragment,
			string withClauseJoinAlias,
			string withRootAlias)
		{
			var joinFragment = new ANSIJoinFragment();

			var on = withClauseFragment ?? SqlString.Empty;
			//Note: filters logic commented as GetOnCondition always returns empty string for entity join (as IsReferenceToPrimaryKey is always true)
			//TODO: If filters for entity join really make sense we need to inline GetOnCondition part that retrieves filters
//			var filters = _entityType.GetOnCondition(_tableAlias, Factory, enabledFilters);
//			if (!string.IsNullOrEmpty(filters))
//			{
//				on = on.Append(filters);
//			}
			joinFragment.AddJoin(_tableName, _tableAlias, Array.Empty<string>(), Array.Empty<string>(), _joinType, on);
			return joinFragment;
		}
	}
}
