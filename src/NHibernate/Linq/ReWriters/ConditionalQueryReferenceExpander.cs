using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using NHibernate.Linq.Clauses;
using NHibernate.Linq.Visitors;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;

namespace NHibernate.Linq.ReWriters
{
	/// <summary>
	/// Expands conditional and coalesce expressions that are merging QueryReferences so that they can be followed by
	/// Member or Method calls.
	/// Ex) query.Where(x => (x.OptionA ?? x.OptionB).Value == value);
	///     query.Where(x => (x.UseA ? x.OptionA : x.OptionB).Value = value);
	/// </summary>
	internal class ConditionalQueryReferenceExpander : NhQueryModelVisitorBase
	{
		private readonly ConditionalQueryReferenceExpressionExpander _expander;

		private ConditionalQueryReferenceExpander()
		{
			_expander = new ConditionalQueryReferenceExpressionExpander();
		}

		public override void VisitSelectClause(SelectClause selectClause, QueryModel queryModel)
		{
			_expander.Transform(selectClause);
		}

		public override void VisitMainFromClause(MainFromClause fromClause, QueryModel queryModel)
		{
			if (fromClause.FromExpression is SubQueryExpression subqueryExpression)
			{
				VisitQueryModel(subqueryExpression.QueryModel);
			}
		}

		public override void VisitOrdering(Ordering ordering, QueryModel queryModel, OrderByClause orderByClause, int index)
		{
			_expander.Transform(ordering);
		}

		public override void VisitResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, int index)
		{
			_expander.Transform(resultOperator);
		}

		public override void VisitWhereClause(WhereClause whereClause, QueryModel queryModel, int index)
		{
			_expander.Transform(whereClause);
		}

		public override void VisitNhHavingClause(NhHavingClause havingClause, QueryModel queryModel, int index)
		{
			_expander.Transform(havingClause);
		}

		public static void ReWrite(QueryModel queryModel)
		{
			var visitor = new ConditionalQueryReferenceExpander();
			visitor.VisitQueryModel(queryModel);
		}

		private class ConditionalQueryReferenceExpressionExpander : RelinqExpressionVisitor
		{
			public void Transform(IClause clause)
			{
				clause.TransformExpressions(Visit);
			}

			public void Transform(Ordering ordering)
			{
				ordering.TransformExpressions(Visit);
			}

			public void Transform(ResultOperatorBase resultOperator)
			{
				resultOperator.TransformExpressions(Visit);
			}

			protected override Expression VisitMember(MemberExpression node)
			{
				var result = (MemberExpression) base.VisitMember(node);
				if (ShouldRewrite(result.Expression))
				{
					return ConditionalQueryReferenceMemberExpressionRewriter.Rewrite(result.Expression, node);
				}
				return result;
			}

			protected override Expression VisitMethodCall(MethodCallExpression node)
			{
				var result = (MethodCallExpression) base.VisitMethodCall(node);
				var isExtension = node.Method.GetCustomAttributes<ExtensionAttribute>().Any();
				var methodObject = isExtension ? node.Arguments[0] : node.Object;

				if (ShouldRewrite(methodObject))
				{
					return ConditionalQueryReferenceMethodCallExpressionRewriter.Rewrite(methodObject, node);
				}
				return result;
			}

			private bool ShouldRewrite(Expression expr, System.Type queryType = null)
			{
				if (expr == null)
				{
					return false;
				}

				// Strip Converts
				while (expr.NodeType == ExpressionType.Convert || expr.NodeType == ExpressionType.ConvertChecked)
				{
					expr = ((UnaryExpression) expr).Operand;
				}

				if (expr is QuerySourceReferenceExpression && queryType?.IsAssignableFrom(expr.Type) == true)
				{
					return true;
				}

				queryType = queryType ?? expr.Type;

				if (expr.NodeType == ExpressionType.Coalesce && expr is BinaryExpression coalesce)
				{
					return ShouldRewrite(coalesce.Left, queryType) && ShouldRewrite(coalesce.Right, queryType);
				}

				if (expr.NodeType == ExpressionType.Conditional && expr is ConditionalExpression conditional)
				{
					return ShouldRewrite(conditional.IfFalse, queryType) && ShouldRewrite(conditional.IfTrue, queryType);
				}

				return false;
			}
		}

		private abstract class ConditionalQueryReferenceExpressionRewriter<T, TVisitor> : RelinqExpressionVisitor
			where T : Expression
			where TVisitor : ConditionalQueryReferenceExpressionRewriter<T, TVisitor>, new()
		{
			protected T OuterExpr { get; private set; }

			private bool _skipUpdate;
			private System.Type _queryType;

			protected override Expression VisitBinary(BinaryExpression node)
			{
				if (node.NodeType != ExpressionType.Coalesce)
				{
					return base.VisitBinary(node);
				}

				// Coalesce expressions must be rewritten to conditionals to keep their logical meaning
				// (x ?? y).Prop --> x != null ? x.Prop : y.Prop
				return Expression.Condition(
					Expression.NotEqual(node.Left, Expression.Constant(null, node.Left.Type)),
					Visit(node.Left),
					Visit(node.Right));
			}

			protected override Expression VisitConditional(ConditionalExpression node)
			{
				_skipUpdate = true;
				var test = Visit(node.Test);
				_skipUpdate = false;
				return Expression.Condition(test, Visit(node.IfTrue), Visit(node.IfFalse));
			}

			protected override Expression VisitQuerySourceReference(QuerySourceReferenceExpression expression)
			{
				if (!_skipUpdate && _queryType.IsAssignableFrom(expression.Type))
				{
					return Rewrite(expression);
				}

				return base.VisitQuerySourceReference(expression);
			}

			protected abstract T Rewrite(QuerySourceReferenceExpression expression);

			public static Expression Rewrite(Expression expression, T outerExpr)
			{
				var visitor = new TVisitor { OuterExpr = outerExpr, _queryType = expression.Type };
				return visitor.Visit(expression);
			}
		}

		private class ConditionalQueryReferenceMemberExpressionRewriter : ConditionalQueryReferenceExpressionRewriter<MemberExpression, ConditionalQueryReferenceMemberExpressionRewriter>
		{
			protected override MemberExpression Rewrite(QuerySourceReferenceExpression expression)
			{
				return Expression.MakeMemberAccess(expression, OuterExpr.Member);
			}
		}

		private class ConditionalQueryReferenceMethodCallExpressionRewriter : ConditionalQueryReferenceExpressionRewriter<MethodCallExpression, ConditionalQueryReferenceMethodCallExpressionRewriter>
		{
			protected override MethodCallExpression Rewrite(QuerySourceReferenceExpression expression)
			{
				var isExtension = OuterExpr.Method.GetCustomAttributes<ExtensionAttribute>().Any();
				if (isExtension)
				{
					var argList = OuterExpr.Arguments.ToArray();
					argList[0] = expression;
					return Expression.Call(null, OuterExpr.Method, argList);
				}

				return Expression.Call(expression, OuterExpr.Method, OuterExpr.Arguments);
			}
		}
	}
}
