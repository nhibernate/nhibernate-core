using Antlr.Runtime;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	internal class EntityJoinFromElement : FromElement
	{
		public EntityJoinFromElement(FromClause fromClause, IQueryable entityPersister, JoinType joinType, string alias)
			:base(new CommonToken(HqlSqlWalker.ENTITY_JOIN, entityPersister.TableName))
		{
			string tableAlias = fromClause.AliasGenerator.CreateName(entityPersister.EntityName);

			EntityType entityType = (EntityType) entityPersister.Type;
			InitializeEntity(fromClause, entityPersister.EntityName, entityPersister, entityType, alias, tableAlias);

			JoinSequence = new EntityJoinJoinSequenceImpl(
				SessionFactoryHelper.Factory,
				entityType,
				tableAlias,
				joinType);

			fromClause.Walker.AddQuerySpaces(entityPersister.QuerySpaces);
		}
	}
}
