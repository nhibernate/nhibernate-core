using System;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Engine.Query;
using NHibernate.Hql.Ast.ANTLR.Tree;

namespace NHibernate.Linq
{
	internal class NhLinqExpressionCache : IQueryExpression
	{
		internal NhLinqExpressionCache(NhLinqExpression expression)
		{
			ExpressionToHqlTranslationResults = expression.ExpressionToHqlTranslationResults ?? throw new ArgumentException("NhLinqExpression is not translated");
			Key = expression.Key;
			Type = expression.Type;
			ParameterDescriptors = expression.ParameterDescriptors;
		}

		public ExpressionToHqlTranslationResults ExpressionToHqlTranslationResults { get; }
		public string Key { get; }
		public System.Type Type { get; }
		public IList<NamedParameterDescriptor> ParameterDescriptors { get; }

		public IASTNode Translate(ISessionFactoryImplementor sessionFactory, bool filter)
		{
			return ExpressionToHqlTranslationResults.Statement.AstNode;
		}
	}
}
