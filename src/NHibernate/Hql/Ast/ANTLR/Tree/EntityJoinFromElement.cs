using System;
using Antlr.Runtime;
using NHibernate.Engine;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	internal class EntityJoinFromElement : FromElement
	{
		public EntityJoinFromElement(FromClause fromClause, IQueryable entityPersister, JoinType joinType, string alias)
			: base(new CommonToken(HqlSqlWalker.ENTITY_JOIN, entityPersister.TableName))
		{
			string tableAlias = fromClause.AliasGenerator.CreateName(entityPersister.EntityName);

			EntityType entityType = (EntityType) entityPersister.Type;
			InitializeEntity(fromClause, entityPersister.EntityName, entityPersister, entityType, alias, tableAlias);

			//NH Specific: hibernate uses special class EntityJoinJoinSequenceImpl
			JoinSequence = new JoinSequence(SessionFactoryHelper.Factory) { ForceFilter = true }
				.AddJoin(entityType, tableAlias, joinType, Array.Empty<string>());

			fromClause.Walker.AddQuerySpaces(entityPersister);
		}
	}
}
