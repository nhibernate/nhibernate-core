using System;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.SqlCommand;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	internal class JoinSubqueryJoinSequenceImpl : JoinSequence
	{
		private readonly string _tableAlias;
		private readonly JoinType _joinType;

		public JoinSubqueryJoinSequenceImpl(ISessionFactoryImplementor factory, string tableAlias, JoinType joinType) : base(factory)
		{
			_tableAlias = tableAlias;
			_joinType = joinType;
		}

		internal override JoinFragment ToJoinFragment(
			IDictionary<string, IFilter> enabledFilters,
			bool includeAllSubclassJoins,
			bool renderSubclassJoins,
			SqlString withClauseFragment)
		{
			var joinFragment = new QueryJoinFragment(Factory.Dialect, false);
			// The query will be added in the next phase, when generating sql
			joinFragment.AddJoin(
				"({query})",
				_tableAlias,
				Array.Empty<string>(),
				Array.Empty<string>(),
				_joinType,
				withClauseFragment ?? SqlString.Empty);
			return joinFragment;
		}
	}
}
