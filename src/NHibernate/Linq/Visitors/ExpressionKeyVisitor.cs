using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using NHibernate.Param;
using Remotion.Linq.Parsing;

namespace NHibernate.Linq.Visitors
{
	/// <summary>
	/// Performs the equivalent of a ToString() on an expression. Swaps out constants for 
	/// parameters so that, for example:
	///		from c in Customers where c.City = "London"
	/// generate the same key as 
	///		from c in Customers where c.City = "Madrid"
	/// </summary>
	public class ExpressionKeyVisitor : ExpressionTreeVisitor
	{
		private readonly IDictionary<ConstantExpression, NamedParameter> _constantToParameterMap;
		readonly StringBuilder _string = new StringBuilder();

		private ExpressionKeyVisitor(IDictionary<ConstantExpression, NamedParameter> constantToParameterMap)
		{
			_constantToParameterMap = constantToParameterMap;
		}

		public static string Visit(Expression expression, IDictionary<ConstantExpression, NamedParameter> parameters)
		{
			var visitor = new ExpressionKeyVisitor(parameters);

			visitor.VisitExpression(expression);

			return visitor.ToString();
		}

		public override string ToString()
		{
			return _string.ToString();
		}

		protected override Expression VisitBinaryExpression(BinaryExpression expression)
		{
			if (expression.Method != null)
			{
				_string.Append(expression.Method.DeclaringType.Name);
				_string.Append(".");
				VisitMethod(expression.Method);
			}
			else
			{
				_string.Append(expression.NodeType);
			}

			_string.Append("(");

			VisitExpression(expression.Left);
			_string.Append(", ");
			VisitExpression(expression.Right);

			_string.Append(")");

			return expression;
		}

		protected override Expression VisitConditionalExpression(ConditionalExpression expression)
		{
			VisitExpression(expression.Test);
			_string.Append(" ? ");
			VisitExpression(expression.IfTrue);
			_string.Append(" : ");
			VisitExpression(expression.IfFalse);

			return expression;
		}

		protected override Expression VisitConstantExpression(ConstantExpression expression)
		{
			NamedParameter param;

			if (_constantToParameterMap.TryGetValue(expression, out param) && insideSelectClause == false)
			{
				// Nulls generate different query plans.  X = variable generates a different query depending on if variable is null or not.
				if (param.Value == null)
					_string.Append("NULL");
				if (param.Value is IEnumerable && !((IEnumerable)param.Value).Cast<object>().Any())
					_string.Append("EmptyList");
				else
					_string.Append(param.Name);
			}
			else
			{
				if (expression.Value == null)
					_string.Append("NULL");
				else
					_string.Append(expression.Value);
			}

			return base.VisitConstantExpression(expression);
		}

		private T AppendCommas<T>(T expression) where T : Expression
		{
			VisitExpression(expression);
			_string.Append(", ");

			return expression;
		}

		protected override Expression VisitLambdaExpression(LambdaExpression expression)
		{
			_string.Append('(');

			VisitList(expression.Parameters, AppendCommas);
			_string.Append(") => (");
			VisitExpression(expression.Body);
			_string.Append(')');

			return expression;
		}

		protected override Expression VisitMemberExpression(MemberExpression expression)
		{
			base.VisitMemberExpression(expression);

			_string.Append('.');
			_string.Append(expression.Member.Name);

			return expression;
		}

		private bool insideSelectClause;
		protected override Expression VisitMethodCallExpression(MethodCallExpression expression)
		{
			var old = insideSelectClause;

			switch (expression.Method.Name)
			{
				case "First":
				case "FirstOrDefault":
				case "Single":
				case "SingleOrDefault":
				case "Select":
					insideSelectClause = true;
					break;
				default:
					insideSelectClause = false;
					break;
			}

			VisitExpression(expression.Object);
			_string.Append('.');
			VisitMethod(expression.Method);
			_string.Append('(');
			VisitList(expression.Arguments, AppendCommas);
			_string.Append(')');

			insideSelectClause = old;
			return expression;
		}

		protected override Expression VisitNewExpression(NewExpression expression)
		{
			_string.Append("new ");
			_string.Append(expression.Constructor.DeclaringType.Name);
			_string.Append('(');
			VisitList(expression.Arguments, AppendCommas);
			_string.Append(')');

			return expression;
		}

		protected override Expression VisitParameterExpression(ParameterExpression expression)
		{
			_string.Append(expression.Name);

			return expression;
		}

		protected override Expression VisitTypeBinaryExpression(TypeBinaryExpression expression)
		{
			_string.Append("IsType(");
			VisitExpression(expression.Expression);
			_string.Append(", ");
			_string.Append(expression.TypeOperand.FullName);
			_string.Append(")");

			return expression;
		}

		protected override Expression VisitUnaryExpression(UnaryExpression expression)
		{
			_string.Append(expression.NodeType);
			_string.Append('(');
			VisitExpression(expression.Operand);
			_string.Append(')');

			return expression;
		}

		protected override Expression VisitQuerySourceReferenceExpression(Remotion.Linq.Clauses.Expressions.QuerySourceReferenceExpression expression)
		{
			_string.Append(expression.ReferencedQuerySource.ItemName);
			return expression;
		}

		private void VisitMethod(MethodInfo methodInfo)
		{
			_string.Append(methodInfo.Name);
			if (methodInfo.IsGenericMethod)
			{
				_string.Append('[');
				_string.Append(string.Join(",", methodInfo.GetGenericArguments().Select(a => a.FullName).ToArray()));
				_string.Append(']');
			}
		}
	}
}