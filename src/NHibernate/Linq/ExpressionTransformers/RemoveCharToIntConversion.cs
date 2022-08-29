using System;
using System.Linq.Expressions;
using Remotion.Linq.Parsing.ExpressionVisitors.Transformation;

namespace NHibernate.Linq.ExpressionTransformers
{
	/// <summary>
	/// Remove unwanted char-to-int conversions in binary expressions
	/// </summary>
	/// <remarks>
	/// The LINQ expression tree may contain unwanted type conversions that were not in the original expression written by the user. For example,
	/// <c>list.Where(someChar => someChar == 'A')</c> becomes the equivalent of <c>list.Where(someChar => (int)someChar == 55)</c> in the expression
	/// tree. Converting this directly to a HQL/SQL statement would yield <code>CAST(x AS INT)</code> which does not work in MSSQLSERVER, and possibly
	/// other databases.
	/// </remarks> 
	public class RemoveCharToIntConversion : IExpressionTransformer<BinaryExpression>
	{
		private static readonly ExpressionType[] _supportedExpressionTypes = new[]
			{
				ExpressionType.Equal,
				ExpressionType.NotEqual,
				ExpressionType.GreaterThan,
				ExpressionType.GreaterThanOrEqual,
				ExpressionType.LessThan,
				ExpressionType.LessThanOrEqual
			};

		public Expression Transform(BinaryExpression expression)
		{
			var lhs = expression.Left;
			var rhs = expression.Right;

			var lhsIsConvertExpression = IsConvertExpression(lhs);
			var rhsIsConvertExpression = IsConvertExpression(rhs);

			if (!lhsIsConvertExpression && !rhsIsConvertExpression) return expression;

			// Variables are not converted to constants (E.g: o.CharProperty == charVariable)
			if (lhsIsConvertExpression && rhsIsConvertExpression)
			{
				var lhsConvertExpression = (UnaryExpression) lhs;
				var rhsConvertExpression = (UnaryExpression) rhs;
				if (!IsConvertCharToInt(lhsConvertExpression) || !IsConvertCharToInt(rhsConvertExpression))
				{
					return expression;
				}

				return Expression.MakeBinary(expression.NodeType, lhsConvertExpression.Operand, rhsConvertExpression.Operand);
			}

			var lhsIsConstantExpression = IsConstantExpression(lhs);
			var rhsIsConstantExpression = IsConstantExpression(rhs);

			if (!lhsIsConstantExpression && !rhsIsConstantExpression) return expression;

			var convertExpression = lhsIsConvertExpression ? (UnaryExpression) lhs : (UnaryExpression) rhs;
			var constantExpression = lhsIsConstantExpression ? (ConstantExpression) lhs : (ConstantExpression) rhs;

			if (IsConvertCharToInt(convertExpression) && constantExpression.Type == typeof(int))
			{
				var constant = Expression.Constant(Convert.ToChar((int) constantExpression.Value));

				if (rhsIsConstantExpression)
					return Expression.MakeBinary(expression.NodeType, convertExpression.Operand, constant);

				return Expression.MakeBinary(expression.NodeType, constant, convertExpression.Operand);
			}

			return expression;
		}

		private static bool IsConvertCharToInt(UnaryExpression expression)
		{
			return expression.Type == typeof(int) && expression.Operand.Type == typeof(char);
		}

		private static bool IsConvertExpression(Expression expression)
		{
			return (expression.NodeType == ExpressionType.Convert);
		}

		private static bool IsConstantExpression(Expression expression)
		{
			return (expression.NodeType == ExpressionType.Constant);
		}

		public ExpressionType[] SupportedExpressionTypes
		{
			get { return _supportedExpressionTypes; }
		}
	}
}
