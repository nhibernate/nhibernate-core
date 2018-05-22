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
	internal class GroupKeyNominator : RelinqExpressionVisitor
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
			return new GroupKeyNominator().Visit(expr);
		}

		public override Expression Visit(Expression expression)
		{
			_depth++;
			var expr = base.Visit(expression);
			_depth--;

			// At the root expression, wrap it in the nominator expression if needed
			if (_depth == 0 && !_transformed && _requiresRootNomination)
			{
				expr = new NhNominatedExpression(expr);
			}
			return expr;
		}

		protected override Expression VisitNewArray(NewArrayExpression expression)
		{
			_transformed = true;
			// Transform each initializer recursively (to allow for nested initializers)
			return Expression.NewArrayInit(expression.Type.GetElementType(), expression.Expressions.Select(VisitInternal));
		}

		protected override Expression VisitNew(NewExpression expression)
		{
			_transformed = true;
			// Transform each initializer recursively (to allow for nested initializers)
			if(expression.Members == null)
				return Expression.New(expression.Constructor, expression.Arguments.Select(VisitInternal));

			return Expression.New(expression.Constructor, expression.Arguments.Select(VisitInternal), expression.Members);
		}

		protected override Expression VisitQuerySourceReference(QuerySourceReferenceExpression expression)
		{
			// If the (sub)expression contains a QuerySourceReference, then the entire expression should be nominated
			_requiresRootNomination = true;
			return base.VisitQuerySourceReference(expression);
		}

		protected override Expression VisitSubQuery(SubQueryExpression expression)
		{
			// If the (sub)expression contains a QuerySourceReference, then the entire expression should be nominated
			_requiresRootNomination = true;
			return base.VisitSubQuery(expression);
		}

		protected override Expression VisitBinary(BinaryExpression expression)
		{
			if (expression.NodeType != ExpressionType.ArrayIndex) 
				return base.VisitBinary(expression);
			
			// If we encounter an array index then we need to attempt to flatten it before nomination
			var flattenedExpression = new ArrayIndexExpressionFlattener().Visit(expression);
			if (flattenedExpression != expression)
				return base.Visit(flattenedExpression);

			return base.VisitBinary(expression);
		}
	}
}
