using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Engine;
using NHibernate.Engine.Query;
using NHibernate.Hql.Ast.ANTLR.Tree;
using NHibernate.Linq.Visitors;
using NHibernate.Param;
using NHibernate.Type;

namespace NHibernate.Linq
{
	public class NhLinqExpression : IQueryExpression
	{
		public string Key { get; private set; }

		public System.Type Type { get; private set; }

		public IList<NamedParameterDescriptor> ParameterDescriptors { get; private set; }

		public NhLinqExpressionReturnType ReturnType { get; private set; }

		public IDictionary<string, Tuple<object, IType>> ParameterValuesByName { get; private set; }

		public ExpressionToHqlTranslationResults ExpressionToHqlTranslationResults { get; private set; }

		public NhLinqExpression(Expression expression, ISessionFactoryImplementor sessionFactory)
		{
			var linqExpression = NhPartialEvaluatingExpressionTreeVisitor.EvaluateIndependentSubtrees(expression);

			// We want logging to be as close as possible to the original expression sent from the
			// application. But if we log before partial evaluation, the log won't include e.g.
			// subquery expressions if those are defined by the application in a variable referenced
			// from the main query.
			LinqLogging.LogExpression("Expression (partially evaluated)", linqExpression);

			var constantToParameterMap = ExpressionParameterVisitor.Visit(linqExpression, sessionFactory);

			ParameterValuesByName = constantToParameterMap.Values.ToDictionary(p => p.Name,
																				p => System.Tuple.Create(p.Value, p.Type));

			Key = ExpressionKeyVisitor.Visit(linqExpression, constantToParameterMap);

			Type = linqExpression.Type;

			// Note - re-linq handles return types via the GetOutputDataInfo method, and allows for SingleOrDefault here for the ChoiceResultOperator...
			ReturnType = NhLinqExpressionReturnType.Scalar;

			if (typeof(IQueryable).IsAssignableFrom(Type))
			{
				Type = Type.GetGenericArguments()[0];
				ReturnType = NhLinqExpressionReturnType.Sequence;
			}

			var requiredHqlParameters = new List<NamedParameterDescriptor>();
			var querySourceNamer = new QuerySourceNamer();
			var queryModel = NhRelinqQueryParser.Parse(linqExpression);
			var visitorParameters = new VisitorParameters(sessionFactory, constantToParameterMap, requiredHqlParameters, querySourceNamer);

			ExpressionToHqlTranslationResults = QueryModelVisitor.GenerateHqlQuery(queryModel, visitorParameters, true);

			ParameterDescriptors = requiredHqlParameters.AsReadOnly();
		}

		public IASTNode Translate(ISessionFactoryImplementor sessionFactory, bool filter)
		{
			return ExpressionToHqlTranslationResults.Statement.AstNode;
		}

		internal void CopyExpressionTranslation(NhLinqExpression other)
		{
			ExpressionToHqlTranslationResults = other.ExpressionToHqlTranslationResults;
			ParameterDescriptors = other.ParameterDescriptors;
		}
	}
}