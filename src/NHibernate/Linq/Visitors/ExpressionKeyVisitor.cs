using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using NHibernate.Engine;
using NHibernate.Param;
using NHibernate.Type;
using NHibernate.Util;
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
	public class ExpressionKeyVisitor : RelinqExpressionVisitor
	{
		private readonly IDictionary<ConstantExpression, NamedParameter> _constantToParameterMap;
		private readonly ISessionFactoryImplementor _sessionFactory;
		readonly StringBuilder _string = new StringBuilder();

		private ExpressionKeyVisitor(
			IDictionary<ConstantExpression, NamedParameter> constantToParameterMap,
			ISessionFactoryImplementor sessionFactory)
		{
			_constantToParameterMap = constantToParameterMap;
			_sessionFactory = sessionFactory;
		}

		// Since v5.3
		[Obsolete("Use the overload with ISessionFactoryImplementor parameter")]
		public static string Visit(Expression expression, IDictionary<ConstantExpression, NamedParameter> parameters)
		{
			var visitor = new ExpressionKeyVisitor(parameters, null);

			visitor.Visit(expression);

			return visitor.ToString();
		}

		/// <summary>
		/// Generates the key for the expression.
		/// </summary>
		/// <param name="rootExpression">The expression.</param>
		/// <param name="sessionFactory">The session factory.</param>
		/// <param name="parameters">Parameters found in <paramref name="rootExpression"/>.</param>
		/// <returns>The key for the expression.</returns>
		public static string Visit(
			Expression rootExpression,
			IDictionary<ConstantExpression, NamedParameter> parameters,
			ISessionFactoryImplementor sessionFactory)
		{
			var visitor = new ExpressionKeyVisitor(parameters, sessionFactory);
			visitor.Visit(rootExpression);

			return visitor.ToString();
		}

		public override string ToString()
		{
			return _string.ToString();
		}

		protected override Expression VisitBinary(BinaryExpression expression)
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

			Visit(expression.Left);
			_string.Append(", ");
			Visit(expression.Right);

			_string.Append(")");

			return expression;
		}

		protected override Expression VisitConditional(ConditionalExpression expression)
		{
			Visit(expression.Test);
			_string.Append(" ? ");
			Visit(expression.IfTrue);
			_string.Append(" : ");
			Visit(expression.IfFalse);

			return expression;
		}

		protected override Expression VisitConstant(ConstantExpression expression)
		{
			NamedParameter param;

			if (_constantToParameterMap == null)
				throw new InvalidOperationException("Cannot visit a constant without a constant to parameter map.");
			if (_constantToParameterMap.TryGetValue(expression, out param))
			{
				VisitParameter(param);
			}
			else
			{
				VisitConstantValue(expression.Value);
			}

			return base.VisitConstant(expression);
		}

		private void VisitConstantValue(object value)
		{
			if (value == null)
			{
				_string.Append("NULL");
				return;
			}

			if (value is IEnumerable enumerable && !(value is IQueryable))
			{
				_string.Append("{");
				_string.Append(string.Join(",", enumerable.Cast<object>()));
				_string.Append("}");
				return;
			}

			// When MappedAs is used we have to put all sql types information in the key in order to
			// distinct when different precisions/sizes are used.
			if (_sessionFactory != null && value is IType type)
			{
				_string.Append(type.Name);
				_string.Append('[');
				_string.Append(string.Join(",", type.SqlTypes(_sessionFactory).Select(o => o.ToString())));
				_string.Append(']');
				return;
			}

			_string.Append(value);
		}

		private void VisitParameter(NamedParameter param)
		{
			// Nulls generate different query plans.  X = variable generates a different query depending on if variable is null or not.
			if (param.Value == null)
			{
				_string.Append("NULL");
				return;
			}

			if (param.IsCollection && !((IEnumerable) param.Value).Cast<object>().Any())
			{
				_string.Append("EmptyList");
			}
			else
			{
				_string.Append(param.Name);
			}
			
			// Add the type in order to avoid invalid parameter conversions (string -> char)
			_string.Append("<");
			_string.Append(param.Value.GetType());
			_string.Append(">");
		}

		private T AppendCommas<T>(T expression) where T : Expression
		{
			Visit(expression);
			_string.Append(", ");

			return expression;
		}

		protected override Expression VisitLambda<T>(Expression<T> expression)
		{
			_string.Append('(');

			Visit(expression.Parameters, AppendCommas);
			_string.Append(") => (");
			Visit(expression.Body);
			_string.Append(')');

			return expression;
		}

		protected override Expression VisitMember(MemberExpression expression)
		{
			base.VisitMember(expression);

			_string.Append('.');
			_string.Append(expression.Member.Name);

			return expression;
		}

#if NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
		protected override Expression VisitInvocation(InvocationExpression expression)
		{
			if (ExpressionsHelper.TryGetDynamicMemberBinder(expression, out var memberBinder))
			{
				Visit(expression.Arguments[1]);
				FormatBinder(memberBinder);
				return expression;
			}

			return base.VisitInvocation(expression);
		}
#endif

		protected override Expression VisitMethodCall(MethodCallExpression expression)
		{
			Visit(expression.Object);
			_string.Append('.');
			VisitMethod(expression.Method);
			_string.Append('(');
			ExpressionVisitor.Visit(expression.Arguments, AppendCommas);
			_string.Append(')');

			return expression;
		}

		protected override Expression VisitNew(NewExpression expression)
		{
			_string.Append("new ");
			_string.Append(expression.Constructor.DeclaringType.AssemblyQualifiedName);
			_string.Append('(');
			Visit(expression.Arguments, AppendCommas);
			_string.Append(')');

			return expression;
		}

		protected override Expression VisitParameter(ParameterExpression expression)
		{
			_string.Append(expression.Name);

			return expression;
		}

		protected override Expression VisitTypeBinary(TypeBinaryExpression expression)
		{
			_string.Append("IsType(");
			Visit(expression.Expression);
			_string.Append(", ");
			_string.Append(expression.TypeOperand.AssemblyQualifiedName);
			_string.Append(")");

			return expression;
		}

		protected override Expression VisitUnary(UnaryExpression expression)
		{
			_string.Append(expression.NodeType);
			_string.Append('(');
			Visit(expression.Operand);
			_string.Append(')');

			return expression;
		}

		protected override Expression VisitQuerySourceReference(Remotion.Linq.Clauses.Expressions.QuerySourceReferenceExpression expression)
		{
			_string.Append(expression.ReferencedQuerySource.ItemName);
			return expression;
		}

		protected override Expression VisitDynamic(DynamicExpression expression)
		{
			Visit(expression.Arguments, AppendCommas);
			FormatBinder(expression.Binder);
			return expression;
		}

		private void VisitMethod(MethodInfo methodInfo)
		{
			_string.Append(methodInfo.Name);
			if (methodInfo.IsGenericMethod)
			{
				_string.Append('[');
				_string.Append(string.Join(",", methodInfo.GetGenericArguments().Select(a => a.AssemblyQualifiedName)));
				_string.Append(']');
			}
		}

		private void FormatBinder(CallSiteBinder binder)
		{
			switch (binder)
			{
				case ConvertBinder b:
					_string.Append("Convert ").Append(b.Type);
					break;
				case CreateInstanceBinder _:
					_string.Append("Create");
					break;
				case DeleteIndexBinder _:
					_string.Append("DeleteIndex");
					break;
				case DeleteMemberBinder b:
					_string.Append("DeleteMember ").Append(b.Name);
					break;
				case BinaryOperationBinder b:
					_string.Append(b.Operation);
					break;
				case GetIndexBinder _:
					_string.Append("GetIndex");
					break;
				case GetMemberBinder b:
					_string.Append("GetMember ").Append(b.Name);
					break;
				case InvokeBinder _:
					_string.Append("Invoke");
					break;
				case InvokeMemberBinder b:
					_string.Append("InvokeMember ").Append(b.Name);
					break;
				case SetIndexBinder _:
					_string.Append("SetIndex");
					break;
				case SetMemberBinder b:
					_string.Append("SetMember ").Append(b.Name);
					break;
				case UnaryOperationBinder b:
					_string.Append(b.Operation);
					break;
				case DynamicMetaObjectBinder _:
					_string.Append("DynamicMetaObject");
					break;
			}
		}
	}
}
