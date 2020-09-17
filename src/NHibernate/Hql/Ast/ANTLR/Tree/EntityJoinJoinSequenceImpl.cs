using System;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	class EntityJoinJoinSequenceImpl : JoinSequence
	{
		public EntityJoinJoinSequenceImpl(ISessionFactoryImplementor factory, EntityType entityType, string tableAlias, JoinType joinType):base(factory)
		{
			AddJoin(entityType, tableAlias, joinType, Array.Empty<string>());
		}
	}
}
