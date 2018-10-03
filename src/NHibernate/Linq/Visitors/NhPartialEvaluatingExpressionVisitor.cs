using System;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Linq.Functions;
using NHibernate.Util;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;
using Remotion.Linq.Parsing.ExpressionVisitors;
using Remotion.Linq.Parsing.ExpressionVisitors.TreeEvaluation;

namespace NHibernate.Linq.Visitors
{
	internal class NhPartialEvaluatingExpressionVisitor : RelinqExpressionVisitor, IPartialEvaluationExceptionExpressionVisitor
	{
		private readonly ISessionFactoryImplementor _sessionFactory;

		internal NhPartialEvaluatingExpressionVisitor(ISessionFactoryImplementor sessionFactory)
		{
			_sessionFactory = sessionFactory;
		}

		protected override Expression VisitConstant(ConstantExpression expression)
		{
			if (expression.Value is Expression value)
			{
				return EvaluateIndependentSubtrees(value, _sessionFactory);
			}

			return base.VisitConstant(expression);
		}

		public static Expression EvaluateIndependentSubtrees(
			Expression expression,
			ISessionFactoryImplementor sessionFactory)
		{
			var evaluatedExpression = PartialEvaluatingExpressionVisitor.EvaluateIndependentSubtrees(
				expression,
				new NhEvaluatableExpressionFilter(sessionFactory));
			return new NhPartialEvaluatingExpressionVisitor(sessionFactory).Visit(evaluatedExpression);
		}

		public Expression VisitPartialEvaluationException(PartialEvaluationExceptionExpression partialEvaluationExceptionExpression)
		{
			throw new HibernateException(
				$"Evaluation failure on {partialEvaluationExceptionExpression.EvaluatedExpression}",
				partialEvaluationExceptionExpression.Exception);
		}
	}

	internal class NhEvaluatableExpressionFilter : EvaluatableExpressionFilterBase
	{
		private readonly ISessionFactoryImplementor _sessionFactory;

		internal NhEvaluatableExpressionFilter(ISessionFactoryImplementor sessionFactory)
		{
			_sessionFactory = sessionFactory;
		}

		public override bool IsEvaluatableConstant(ConstantExpression node)
		{
			if (node.Value is IPersistentCollection && node.Value is IQueryable)
			{
				return false;
			}

			return base.IsEvaluatableConstant(node);
		}

		public override bool IsEvaluatableMember(MemberExpression node)
		{
			if (node == null)
				throw new ArgumentNullException(nameof(node));

			if (_sessionFactory == null || _sessionFactory.Settings.LinqToHqlLegacyPreEvaluation ||
				!_sessionFactory.Settings.LinqToHqlGeneratorsRegistry.TryGetGenerator(node.Member, out var generator))
				return true;

			return generator.AllowPreEvaluation(node.Member, _sessionFactory);
		}

		public override bool IsEvaluatableMethodCall(MethodCallExpression node)
		{
			if (node == null)
				throw new ArgumentNullException(nameof(node));

			var attributes = node.Method
				.GetCustomAttributes(typeof(LinqExtensionMethodAttributeBase), false)
				.ToArray(x => (LinqExtensionMethodAttributeBase) x);
			return attributes.Length == 0 ||
				attributes.Any(a => a.PreEvaluation == LinqExtensionPreEvaluation.AllowPreEvaluation);
		}
	}
}
