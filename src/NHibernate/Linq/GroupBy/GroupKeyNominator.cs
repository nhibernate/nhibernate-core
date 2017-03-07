using System.Linq;
using System.Linq.Expressions;
using NHibernate.Linq.Expressions;
using NHibernate.Linq.ReWriters;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Parsing;

namespace NHibernate.Linq.GroupBy
{
	/// <summary>
	/// This class nominates sub-expression trees on the GroupBy Key expression
	/// for inclusion in the Select clause.
	/// </summary>
	internal class GroupKeyNominator : ExpressionTreeVisitor
	{
		private GroupKeyNominator() { }

		private bool _requiresRootNomination;
		private bool _transformed;
		private int _depth;

		public static Expression Visit(GroupResultOperator groupBy)
		{
			return VisitInternal(groupBy.KeySelector);
		}

		private static Expression VisitInternal(Expression expr)
		{
			return new GroupKeyNominator().VisitExpression(expr);
		}

		public override Expression VisitExpression(Expression expression)
		{
			_depth++;
			var expr = base.VisitExpression(expression);
			_depth--;

			// At the root expression, wrap it in the nominator expression if needed
			if (_depth == 0 && !_transformed && _requiresRootNomination)
			{
				expr = new NhNominatedExpression(expr);
			}
			return expr;
		}

		protected override Expression VisitNewArrayExpression(NewArrayExpression expression)
		{
			_transformed = true;
			// Transform each initializer recursively (to allow for nested initializers)
			return Expression.NewArrayInit(expression.Type.GetElementType(), expression.Expressions.Select(VisitInternal));
		}

		protected override Expression VisitNewExpression(NewExpression expression)
		{
			_transformed = true;
			// Transform each initializer recursively (to allow for nested initializers)
			return Expression.New(expression.Constructor, expression.Arguments.Select(VisitInternal), expression.Members);
		}

		protected override Expression VisitQuerySourceReferenceExpression(QuerySourceReferenceExpression expression)
		{
			// If the (sub)expression contains a QuerySourceReference, then the entire expression should be nominated
			_requiresRootNomination = true;
			return base.VisitQuerySourceReferenceExpression(expression);
		}

		protected override Expression VisitSubQueryExpression(SubQueryExpression expression)
		{
			// If the (sub)expression contains a QuerySourceReference, then the entire expression should be nominated
			_requiresRootNomination = true;
			return base.VisitSubQueryExpression(expression);
		}

		protected override Expression VisitBinaryExpression(BinaryExpression expression)
		{
			if (expression.NodeType != ExpressionType.ArrayIndex) 
				return base.VisitBinaryExpression(expression);
			
			// If we encounter an array index then we need to attempt to flatten it before nomination
			var flattenedExpression = new ArrayIndexExpressionFlattener().VisitExpression(expression);
			if (flattenedExpression != expression)
				return base.VisitExpression(flattenedExpression);

			return base.VisitBinaryExpression(expression);
		}
	}
}