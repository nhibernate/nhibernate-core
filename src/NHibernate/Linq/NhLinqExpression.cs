﻿using System;
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
		public string Key { get; }

		public System.Type Type { get; private set; }

		public IList<NamedParameterDescriptor> ParameterDescriptors { get; private set; }

		public NhLinqExpressionReturnType ReturnType { get; }

		public IDictionary<string, Tuple<object, IType>> ParameterValuesByName { get; }

		public ExpressionToHqlTranslationResults ExpressionToHqlTranslationResults { get; private set; }

		private Expression _expression;
		private readonly IDictionary<ConstantExpression, NamedParameter> _constantToParameterMap;

		public NhLinqExpression(Expression expression, ISessionFactoryImplementor sessionFactory)
		{
			_expression = NhRelinqQueryParser.PreTransform(expression);

			// We want logging to be as close as possible to the original expression sent from the
			// application. But if we log before partial evaluation done in PreTransform, the log won't
			// include e.g. subquery expressions if those are defined by the application in a variable
			// referenced from the main query.
			LinqLogging.LogExpression("Expression (partially evaluated)", _expression);

			_constantToParameterMap = ExpressionParameterVisitor.Visit(ref _expression, sessionFactory);

			ParameterValuesByName = _constantToParameterMap.Values.ToDictionary(p => p.Name,
																				p => System.Tuple.Create(p.Value, p.Type));

			Key = ExpressionKeyVisitor.Visit(_expression, _constantToParameterMap);

			Type = _expression.Type;

			// Note - re-linq handles return types via the GetOutputDataInfo method, and allows for SingleOrDefault here for the ChoiceResultOperator...
			ReturnType = NhLinqExpressionReturnType.Scalar;

			if (typeof(IQueryable).IsAssignableFrom(Type))
			{
				Type = Type.GetGenericArguments()[0];
				ReturnType = NhLinqExpressionReturnType.Sequence;
			}
		}

		public IASTNode Translate(ISessionFactoryImplementor sessionFactory, bool filter)
		{
			var requiredHqlParameters = new List<NamedParameterDescriptor>();
			var querySourceNamer = new QuerySourceNamer();
			var queryModel = NhRelinqQueryParser.Parse(_expression);
			var visitorParameters = new VisitorParameters(sessionFactory, _constantToParameterMap, requiredHqlParameters, querySourceNamer);

			ExpressionToHqlTranslationResults = QueryModelVisitor.GenerateHqlQuery(queryModel, visitorParameters, true, ReturnType);

			if (ExpressionToHqlTranslationResults.ExecuteResultTypeOverride != null)
				Type = ExpressionToHqlTranslationResults.ExecuteResultTypeOverride;

			ParameterDescriptors = requiredHqlParameters.AsReadOnly();
			
			return ExpressionToHqlTranslationResults.Statement.AstNode;
		}

		internal void CopyExpressionTranslation(NhLinqExpression other)
		{
			ExpressionToHqlTranslationResults = other.ExpressionToHqlTranslationResults;
			ParameterDescriptors = other.ParameterDescriptors;
			// Type could have been overridden by translation.
			Type = other.Type;
		}
	}
}
