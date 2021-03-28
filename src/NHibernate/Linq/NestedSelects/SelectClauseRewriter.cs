using System.Collections.Generic;
using System.Linq.Expressions;
using NHibernate.Util;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;

namespace NHibernate.Linq.NestedSelects
{
	class SelectClauseRewriter : RelinqExpressionVisitor
	{
		private readonly Dictionary<Expression, Expression> _dictionary;

		readonly ICollection<ExpressionHolder> expressions;
		readonly Expression parameter;
		readonly int tuple;

		public SelectClauseRewriter(Expression parameter, ICollection<ExpressionHolder> expressions, Expression expression, Dictionary<Expression, Expression> dictionary) 
			: this(parameter, expressions, expression, 0, dictionary)
		{
		}

		public SelectClauseRewriter(Expression parameter, ICollection<ExpressionHolder> expressions, Expression expression, int tuple, Dictionary<Expression, Expression> dictionary)
		{
			this.expressions = expressions;
			this.parameter = parameter;
			this.tuple = tuple;
			this.expressions.Add(new ExpressionHolder { Expression = expression, Tuple = tuple }); //ID placeholder
			_dictionary = dictionary;
		}

		public override Expression Visit(Expression expression)
		{
			if (expression == null)
				return null;
			Expression replacement;
			if (_dictionary.TryGetValue(expression, out replacement))
				return replacement;

			return base.Visit(expression);
		}

		protected override Expression VisitUnary(UnaryExpression node)
		{
			if (node.NodeType == ExpressionType.Convert &&
				// We can skip a convert node only when the underlying types are equal otherwise it
				// will throw an exception when trying to convert the value from an object
				// (e.g. (int?)(Enum?) input[0] -> (Enum?) cast cannot be skipped)
				node.Type.UnwrapIfNullable() == node.Operand.Type.UnwrapIfNullable() &&
				(node.Operand is MemberExpression || node.Operand is QuerySourceReferenceExpression))
			{
				return AddAndConvertExpression(node.Operand, node.Type);
			}

			return base.VisitUnary(node);
		}

		protected override Expression VisitMember(MemberExpression expression)
		{
			return AddAndConvertExpression(expression);
		}

		protected override Expression VisitQuerySourceReference(QuerySourceReferenceExpression expression)
		{
			return AddAndConvertExpression(expression);
		}

		private Expression AddAndConvertExpression(Expression expression)
		{
			return AddAndConvertExpression(expression, expression.Type);
		}

		private Expression AddAndConvertExpression(Expression expression, System.Type type)
		{
			expressions.Add(new ExpressionHolder { Expression = expression, Tuple = tuple });

			return Expression.Convert(
				Expression.ArrayIndex(
					Expression.Property(parameter, Tuple.ItemsProperty),
					Expression.Constant(expressions.Count - 1)),
				type);
		}
	}
}
