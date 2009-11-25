using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Text;
using Remotion.Data.Linq.Parsing;

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
				_string.Append(expression.Method.Name);
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

			if (_constantToParameterMap.TryGetValue(expression, out param))
			{
				_string.Append(param.Name);
			}
			else
			{
				_string.Append(expression.Value);
			}

			return base.VisitConstantExpression(expression);
		}

		protected override ElementInit VisitElementInit(ElementInit elementInit)
		{
			return base.VisitElementInit(elementInit);
		}

		protected override ReadOnlyCollection<T> VisitExpressionList<T>(ReadOnlyCollection<T> expressions)
		{
			if (expressions.Count > 0)
			{
				VisitExpression(expressions[0]);

				for (var i = 1; i < expressions.Count; i++)
				{
					_string.Append(", ");
					VisitExpression(expressions[i]);
				}
			}

			return expressions;
		}

		protected override Expression VisitInvocationExpression(InvocationExpression expression)
		{
			return base.VisitInvocationExpression(expression);
		}

		protected override Expression VisitLambdaExpression(LambdaExpression expression)
		{
			_string.Append('(');
			VisitExpressionList(expression.Parameters);
			_string.Append(") => (");
			VisitExpression(expression.Body);
			_string.Append(')');

			return expression;
		}

		protected override Expression VisitListInitExpression(ListInitExpression expression)
		{
			return base.VisitListInitExpression(expression);
		}

		protected override MemberBinding VisitMemberAssignment(MemberAssignment memberAssigment)
		{
			return base.VisitMemberAssignment(memberAssigment);
		}

		protected override MemberBinding VisitMemberBinding(MemberBinding memberBinding)
		{
			return base.VisitMemberBinding(memberBinding);
		}

		protected override Expression VisitMemberExpression(MemberExpression expression)
		{
			base.VisitMemberExpression(expression);

			_string.Append('.');
			_string.Append(expression.Member.Name);

			return expression;
		}

		protected override Expression VisitMemberInitExpression(MemberInitExpression expression)
		{
			return base.VisitMemberInitExpression(expression);
		}

		protected override MemberBinding VisitMemberListBinding(MemberListBinding listBinding)
		{
			return base.VisitMemberListBinding(listBinding);
		}

		protected override MemberBinding VisitMemberMemberBinding(MemberMemberBinding binding)
		{
			return base.VisitMemberMemberBinding(binding);
		}

		protected override Expression VisitMethodCallExpression(MethodCallExpression expression)
		{
			VisitExpression(expression.Object);
			_string.Append('.');
			_string.Append(expression.Method.Name);
			_string.Append('(');
			VisitExpressionList(expression.Arguments);
			_string.Append(')');

			return expression;
		}

		protected override Expression VisitNewArrayExpression(NewArrayExpression expression)
		{
			return base.VisitNewArrayExpression(expression);
		}

		protected override Expression VisitNewExpression(NewExpression expression)
		{
			_string.Append("new ");
			_string.Append(expression.Constructor.DeclaringType.Name);
			_string.Append('(');
			VisitExpressionList(expression.Arguments);
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
			return base.VisitTypeBinaryExpression(expression);
		}

		protected override Expression VisitUnaryExpression(UnaryExpression expression)
		{
			_string.Append(expression.NodeType);
			_string.Append('(');
			VisitExpression(expression.Operand);
			_string.Append(')');

			return expression;
		}

		protected override Expression VisitUnknownExpression(Expression expression)
		{
			return base.VisitUnknownExpression(expression);
		}
	}
}