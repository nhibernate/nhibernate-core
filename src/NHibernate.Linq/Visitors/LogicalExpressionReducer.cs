using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace NHibernate.Linq.Visitors
{
	/// <summary>
	/// Reduces the expression using logical simplification
	/// Please note that it doesn't evaluate the binary expressions with constant operands
	/// </summary>

	//May be used for animal.Offspring.Any() == true
	//Eventhough it is almost unnecessary to reduce not(not(true)) kind of expression, but simplified expressions are gold.
	public class LogicalExpressionReducer:ExpressionVisitor
	{
		protected override Expression VisitBinary(BinaryExpression expr)
		{
			bool modified;
			Expression e = ProcessBinaryExpression(expr.Left, expr.Right, expr.NodeType, out modified);
			if (modified)
				return Visit(e);
			e = ProcessBinaryExpression(expr.Right, expr.Left, expr.NodeType, out modified);
			if(modified)
				return Visit(e);
			return expr;

		}

		protected override Expression VisitUnary(UnaryExpression u)
		{
			switch(u.NodeType)
			{
				case ExpressionType.Not:
					if (u.Operand.NodeType == ExpressionType.Not)
						return Visit(((UnaryExpression) (u.Operand)).Operand);
					else if (u.Operand.NodeType == ExpressionType.Equal)
					{
						var binaryExpression = u.Operand as BinaryExpression;
						return Visit(Expression.NotEqual(binaryExpression.Left, binaryExpression.Right));
					}
					else if (u.Operand.NodeType == ExpressionType.Equal)
					{
						var binaryExpression = u.Operand as BinaryExpression;
						return Visit(Expression.Equal(binaryExpression.Left, binaryExpression.Right));
					}
					else if(u.Operand.NodeType==ExpressionType.Constant)
					{
						var constantExpression = u.Operand as ConstantExpression;
						return Visit(Expression.Constant(!((bool)constantExpression.Value)));
					}
					break;
			}
			return u;
		}

		private Expression ProcessBinaryExpression(Expression exprToCompare, Expression exprToReturn,
														ExpressionType nodeType, out bool modified)
		{
			modified = false;

			var visitor = new BooleanConstantFinder();
			visitor.Visit(exprToCompare);

			if (!visitor.Constant.HasValue)
				return null;

			bool constantValue = visitor.Constant.Value;
			switch (nodeType)
			{
				case ExpressionType.Equal:
					modified = true;
					return constantValue ? exprToReturn : Expression.Not(exprToReturn);
				case ExpressionType.NotEqual:
					modified = true;
					return constantValue ? Expression.Not(exprToReturn) : exprToReturn;
				case ExpressionType.Or:
				case ExpressionType.OrElse:
					modified = true;
					return constantValue ? Expression.Constant(true) : exprToReturn;
				case ExpressionType.And:
				case ExpressionType.AndAlso:
					modified = true;
					return constantValue ? exprToReturn : Expression.Constant(false);
				default:
					return null;
			}

		}

		class BooleanConstantFinder : ExpressionVisitor
		{
			private bool isNestedBinaryExpression;

			public bool? Constant { get; private set; }

			protected override Expression VisitConstant(ConstantExpression c)
			{
				if (c.Type == typeof(bool) && !isNestedBinaryExpression)
					Constant = (bool)c.Value;
				return c;
			}

			protected override Expression VisitBinary(BinaryExpression b)
			{
				isNestedBinaryExpression = true;
				return b;
			}
		}
	}
}