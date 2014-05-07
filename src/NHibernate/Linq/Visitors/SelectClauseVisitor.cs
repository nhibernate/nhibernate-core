using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Expressions;
using Remotion.Linq.Parsing;

namespace NHibernate.Linq.Visitors
{
	public class SelectClauseVisitor : ExpressionTreeVisitor
	{
		private readonly HqlTreeBuilder _hqlTreeBuilder = new HqlTreeBuilder();
		private HashSet<Expression> _hqlNodes;
		private readonly ParameterExpression _inputParameter;
		private readonly VisitorParameters _parameters;
		private int _iColumn;
		private List<HqlExpression> _hqlTreeNodes = new List<HqlExpression>();
		private readonly HqlGeneratorExpressionTreeVisitor _hqlVisitor;

		public SelectClauseVisitor(System.Type inputType, VisitorParameters parameters)
		{
			_inputParameter = Expression.Parameter(inputType, "input");
			_parameters = parameters;
			_hqlVisitor = new HqlGeneratorExpressionTreeVisitor(_parameters);
		}

		public LambdaExpression ProjectionExpression { get; private set; }

		public IEnumerable<HqlExpression> GetHqlNodes()
		{
			return _hqlTreeNodes;
		}

		public void Visit(Expression expression)
		{
			var distinct = expression as NhDistinctExpression;
			if (distinct != null)
			{
				expression = distinct.Expression;
			}

			// Find the sub trees that can be expressed purely in HQL
			var nominator = new SelectClauseHqlNominator(_parameters);
			nominator.Visit(expression);
			_hqlNodes = nominator.HqlCandidates;

			// Linq2SQL ignores calls to local methods. Linq2EF seems to not support
			// calls to local methods at all. For NHibernate we support local methods,
			// but prevent their use together with server-side distinct, since it may
			// end up being wrong.
			if (distinct != null && nominator.ContainsUntranslatedMethodCalls)
				throw new NotSupportedException("Cannot use distinct on result that depends on methods for which no SQL equivalent exist.");

			// Now visit the tree
			var projection = VisitExpression(expression);

			if ((projection != expression) && !_hqlNodes.Contains(expression))
			{
				ProjectionExpression = Expression.Lambda(projection, _inputParameter);
			}

			// Handle any boolean results in the output nodes
			_hqlTreeNodes = BooleanToCaseConvertor.Convert(_hqlTreeNodes).ToList();

			if (distinct != null)
			{
				var treeNodes = new List<HqlTreeNode>(_hqlTreeNodes.Count + 1) {_hqlTreeBuilder.Distinct()};
				treeNodes.AddRange(_hqlTreeNodes);
				_hqlTreeNodes = new List<HqlExpression>(1) {_hqlTreeBuilder.ExpressionSubTreeHolder(treeNodes)};
			}
		}

		public override Expression VisitExpression(Expression expression)
		{
			if (expression == null)
			{
				return null;
			}

			if (_hqlNodes.Contains(expression))
			{
				// Pure HQL evaluation
				_hqlTreeNodes.Add(_hqlVisitor.Visit(expression).AsExpression());

				return Expression.Convert(Expression.ArrayIndex(_inputParameter, Expression.Constant(_iColumn++)), expression.Type);
			}

			// Can't handle this node with HQL.  Just recurse down, and emit the expression
			return base.VisitExpression(expression);
		}
	}

	public static class BooleanToCaseConvertor
	{
		public static IEnumerable<HqlExpression> Convert(IEnumerable<HqlExpression> hqlTreeNodes)
		{
			return hqlTreeNodes.Select(node => ConvertBooleanToCase(node));
		}

		public static HqlExpression ConvertBooleanToCase(HqlExpression node)
		{
			if (node is HqlBooleanExpression)
			{
				var builder = new HqlTreeBuilder();

				return builder.Case(
					new[] {builder.When(node, builder.True())},
					builder.False());
			}

			return node;
		}
	}
}