using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Remotion.Linq.Clauses.Expressions;
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

		/// <summary>
		/// Generates the key for a child expression based on the <paramref name="parameters"/> of one of its parents.
		/// </summary>
		/// <param name="childExpression">The child expression.</param>
		/// <param name="parameters">The query parameters for the parent expression.</param>
		/// <param name="sessionFactory">The session factory.</param>
		/// <returns>The key for the child expression.</returns>
		internal static string VisitChild(
			Expression childExpression,
			IDictionary<ConstantExpression, NamedParameter> parameters,
			ISessionFactoryImplementor sessionFactory)
		{
			var visitor = new ExpressionKeyVisitor(parameters, sessionFactory);
			visitor.Visit(childExpression);

			return visitor.ToString();
		}

		public override string ToString()
		{
			return _string.ToString();
		}

		protected override Expression VisitBinary(BinaryExpression expression)
		{
			if (expression.NodeType == ExpressionType.ArrayIndex)
			{
				Visit(expression.Left);
				_string.Append("[");
				Visit(expression.Right);
				_string.Append("]");

				return expression;
			}

			_string.Append("(");
			Visit(expression.Left);
			_string.Append(" ");
			_string.Append(expression.NodeType);
			_string.Append(" ");
			Visit(expression.Right);
			_string.Append(")");

			return expression;
		}

		protected override Expression VisitConditional(ConditionalExpression expression)
		{
			_string.Append("IIF(");
			Visit(expression.Test);
			_string.Append(",");
			Visit(expression.IfTrue);
			_string.Append(",");
			Visit(expression.IfFalse);
			_string.Append(")");

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

			if (value is string)
			{
				_string.Append('"');
				_string.Append(value);
				_string.Append('"');
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

			var stringValue = value.ToString();
			if (stringValue == value.GetType().ToString())
			{
				_string.Append("value(");
				_string.Append(stringValue);
				_string.Append(')');
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

		protected override Expression VisitLambda<T>(Expression<T> expression)
		{
			_string.Append('(');

			Visit(expression.Parameters, ',');
			_string.Append(") => (");
			Visit(expression.Body);
			_string.Append(')');

			return expression;
		}

		protected override Expression VisitListInit(ListInitExpression expression)
		{
			Visit(expression.NewExpression);
			_string.Append(" {");
			Visit(expression.Initializers, VisitElementInit, ',');
			_string.Append('}');

			return expression;
		}

		protected override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
		{
			_string.Append('(');
			Visit(node.Variables, ',');
			_string.Append(')');

			return node;
		}

		protected override Expression VisitMember(MemberExpression expression)
		{
			if (expression.Expression != null)
			{
				Visit(expression.Expression);
			}
			else
			{
				// Static members
				_string.Append(expression.Member.DeclaringType.Name);
			}

			_string.Append('.');
			_string.Append(expression.Member.Name);

			return expression;
		}

		protected override Expression VisitMemberInit(MemberInitExpression expression)
		{
			if (expression.NewExpression.Arguments.Count == 0 &&
			    expression.NewExpression.Type.Name.Contains('<'))
			{
				// Anonymous type
				_string.Append("new");
			}
			else
			{
				Visit(expression.NewExpression);
			}

			_string.Append(" {");
			Visit(expression.Bindings, VisitMemberBinding, ',');
			_string.Append('}');

			return expression;
		}

		protected override MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
		{
			_string.Append(assignment.Member.Name);
			_string.Append(" = ");
			Visit(assignment.Expression);

			return assignment;
		}

		protected override MemberListBinding VisitMemberListBinding(MemberListBinding binding)
		{
			_string.Append(binding.Member.Name);
			_string.Append(" = {");
			Visit(binding.Initializers, VisitElementInit, ',');
			_string.Append('}');

			return binding;
		}

		protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding)
		{
			_string.Append(binding.Member.Name);
			_string.Append(" = {");
			Visit(binding.Bindings, VisitMemberBinding, ',');
			_string.Append('}');

			return binding;
		}

		protected override ElementInit VisitElementInit(ElementInit initializer)
		{
			_string.Append(initializer.AddMethod);
			Visit(initializer.Arguments, ',', '(', ')');

			return initializer;
		}

		protected override Expression VisitInvocation(InvocationExpression expression)
		{
#if NETCOREAPP2_0
			if (ExpressionsHelper.TryGetDynamicMemberBinder(expression, out var memberBinder))
			{
				Visit(expression.Arguments[1]);
				FormatBinder(memberBinder);
				return expression;
			}
#endif

			_string.Append("Invoke(");
			Visit(expression.Expression);
			Visit(expression.Arguments, ',', null, ')');

			return expression;
		}

		protected override Expression VisitMethodCall(MethodCallExpression expression)
		{
			if (expression.Object != null)
			{
				Visit(expression.Object);
				_string.Append('.');
			}

			VisitMethod(expression.Method);
			Visit(expression.Arguments, ',', '(', ')');

			return expression;
		}

		protected override Expression VisitNewArray(NewArrayExpression node)
		{
			switch (node.NodeType)
			{
				case ExpressionType.NewArrayBounds:
					// new MyType[](expr1, expr2)
					_string.Append("new ");
					_string.Append(node.Type);
					Visit(node.Expressions, ',', '(', ')');
					break;
				case ExpressionType.NewArrayInit:
					// new [] {expr1, expr2}
					_string.Append("new [] ");
					Visit(node.Expressions, ',', '{', '}');
					break;
			}

			return node;
		}

		protected override Expression VisitNew(NewExpression expression)
		{
			_string.Append("new ");
			_string.Append(GetTypeName(expression.Constructor.DeclaringType));
			var preVisitAction = expression.Members != null
				? i =>
				{
					_string.Append(expression.Members[i].Name);
					_string.Append(" = ");
				}
				: (Action<int>) null;
			Visit(expression.Arguments, ',', '(', ')', preVisitAction);

			return expression;
		}

		protected override Expression VisitParameter(ParameterExpression expression)
		{
			_string.Append(expression.Name);

			return expression;
		}

		protected override Expression VisitTypeBinary(TypeBinaryExpression expression)
		{
			_string.Append("(");
			Visit(expression.Expression);
			switch (expression.NodeType)
			{
				case ExpressionType.TypeIs:
					_string.Append(" Is ");
					break;
				case ExpressionType.TypeEqual:
					_string.Append(" TypeEqual ");
					break;
			}

			_string.Append(GetTypeName(expression.TypeOperand));
			_string.Append(")");

			return expression;
		}

		protected override Expression VisitUnary(UnaryExpression expression)
		{
			_string.Append(expression.NodeType);
			_string.Append('(');
			Visit(expression.Operand);

			switch (expression.NodeType)
			{
				case ExpressionType.TypeAs:
					_string.Append(" As ");
					_string.Append(GetTypeName(expression.Type));
					_string.Append(')');
					break;
				case ExpressionType.Convert:
				case ExpressionType.ConvertChecked:
					_string.Append(", ");
					_string.Append(GetTypeName(expression.Type));
					_string.Append(')');
					break;
				default:
					_string.Append(')');
					break;
			}

			return expression;
		}

		protected override Expression VisitDefault(DefaultExpression node)
		{
			_string.Append("default(");
			_string.Append(GetTypeName(node.Type));
			_string.Append(')');

			return node;
		}

		protected override Expression VisitIndex(IndexExpression node)
		{
			if (node.Object != null)
			{
				Visit(node.Object);
			}
			else
			{
				_string.Append(node.Indexer.DeclaringType.Name);
			}
			if (node.Indexer != null)
			{
				_string.Append('.');
				_string.Append(node.Indexer.Name);
			}

			Visit(node.Arguments, ',', '[', ']');

			return node;
		}

		protected override Expression VisitExtension(Expression expression)
		{
			_string.Append(expression.GetType());
			_string.Append('(');
			base.VisitExtension(expression);
			_string.Append(')');

			return expression;
		}

		protected override Expression VisitQuerySourceReference(QuerySourceReferenceExpression expression)
		{
			_string.Append(expression.ReferencedQuerySource);

			return expression;
		}

		protected override Expression VisitDynamic(DynamicExpression expression)
		{
			Visit(expression.Arguments, ',', '(', ')');
			FormatBinder(expression.Binder);

			return expression;
		}

		protected override Expression VisitSubQuery(SubQueryExpression expression)
		{
			_string.Append("SubQuery");
			_string.Append('(');
			new QueryModelKeyVisitor(this, _string).VisitQueryModel(expression.QueryModel);
			_string.Append(')');

			return expression;
		}

		private void VisitMethod(MethodInfo methodInfo)
		{
			_string.Append(methodInfo.Name);
			if (methodInfo.IsGenericMethod)
			{
				_string.Append('[');
				_string.Append(string.Join(",", methodInfo.GetGenericArguments().Select(GetTypeName)));
				_string.Append(']');
			}
		}

		private void Visit<T>(ReadOnlyCollection<T> nodes, char separator, Action<int> preVisitAction = null)
			where T : Expression
		{
			Visit(nodes, separator, null, null, preVisitAction);
		}

		private void Visit<T>(
			ReadOnlyCollection<T> nodes,
			char separator,
			char? openSymbol,
			char? closeSymbol,
			Action<int> preVisitAction = null)
			where T : Expression
		{
			if (openSymbol.HasValue)
			{
				_string.Append(openSymbol.Value);
			}

			for (var i = 0; i < nodes.Count; i++)
			{
				if (i > 0)
				{
					_string.Append(separator);
				}

				preVisitAction?.Invoke(i);
				Visit(nodes[i]);
			}

			if (closeSymbol.HasValue)
			{
				_string.Append(closeSymbol.Value);
			}
		}

		private void Visit<T>(ReadOnlyCollection<T> nodes, Func<T, T> elementVisitor, char separator)
		{
			for (var i = 0; i < nodes.Count; i++)
			{
				if (i > 0)
				{
					_string.Append(separator);
				}

				elementVisitor(nodes[i]);
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

		internal static string GetTypeName(System.Type type)
		{
			return type.Namespace == "System"
				? type.FullName
				: type.AssemblyQualifiedName;
		}
	}
}
