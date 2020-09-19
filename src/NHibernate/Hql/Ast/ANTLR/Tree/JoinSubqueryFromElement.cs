using System.Linq;
using Antlr.Runtime;
using NHibernate.Engine;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	internal class JoinSubqueryFromElement : FromElement
	{
		public JoinSubqueryFromElement(FromClause fromClause, QueryNode queryNode, JoinType joinType, string alias)
			: base(new CommonToken(HqlSqlWalker.JOIN_SUBQUERY, "join sub-query"))
		{
			var tableAlias = fromClause.AliasGenerator.CreateName("subquery");
			var querySelectClause = queryNode.GetSelectClause();
			var dataType = CreateDataType(querySelectClause, out var entityPersister);
			PropertyMapping = new SubqueryPropertyMapping(dataType, querySelectClause);
			QueryNode = queryNode;
			DataType = dataType;
			foreach (var fromElement in querySelectClause.SelectExpressions.Select(o => o.FromElement)
				.Union(querySelectClause.NonScalarExpressions?.Select(o => o.FromElement) ?? Enumerable.Empty<FromElement>())
				.Union(querySelectClause.CollectionFromElements ?? Enumerable.Empty<FromElement>())
				.Where(o => o != null))
			{
				fromElement.ParentFromElement = this;
			}

			Initialize(fromClause, PropertyMapping, dataType, alias, tableAlias, entityPersister);
			JoinSequence = new JoinSubqueryJoinSequenceImpl(
				SessionFactoryHelper.Factory,
				tableAlias,
				joinType);
		}

		private IType CreateDataType(SelectClause selectClause, out IEntityPersister entityPersister)
		{
			var typeLength = selectClause.QueryReturnTypes.Length;
			if (typeLength == 1)
			{
				entityPersister = selectClause
				                  .SelectExpressions
				                  .Select(o => o.FromElement?.EntityPersister)
				                  .FirstOrDefault(o => o != null);
				return selectClause.QueryReturnTypes[0];
			}

			entityPersister = null;
			return new SubqueryComponentType(selectClause.QueryReturnTypes);
		}

		public override IType SelectType => base.SelectType ?? DataType;

		public QueryNode QueryNode { get; }

		public SubqueryPropertyMapping PropertyMapping { get; }

		public SqlString RenderText(SqlString subQuery, ISessionFactoryImplementor sessionFactory)
		{
			var array = RenderText(sessionFactory).Split("{query}");

			return new SqlString(new object[] {array[0], subQuery, array[1]});
		}

		public override string GetIdentityColumn()
		{
			// Return null for a scalar subquery instead of throwing an exception.
			// The node will be removed in the SelectClause
			if (DataType is SubqueryComponentType && PropertyMapping.GetIdentifiersColumns(TableAlias).Count == 0)
			{
				return null;
			}

			return base.GetIdentityColumn();
		}
	}
}
