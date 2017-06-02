using System.Collections.Generic;
using System.Linq.Expressions;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;

namespace NHibernate.Linq.NestedSelects
{
	class SelectClauseRewriter : RelinqExpressionVisitor
	{
		private readonly Dictionary<Expression, Expression> _dictionary;
		private readonly ICollection<ExpressionHolder> _expressions;
		private readonly Expression _parameter;
		private readonly int _tuple;

		public SelectClauseRewriter(Expression parameter, ICollection<ExpressionHolder> expressions, Expression expression,
			Dictionary<Expression, Expression> dictionary)
			: this(parameter, expressions, expression, 0, dictionary) { }

		public SelectClauseRewriter(Expression parameter, ICollection<ExpressionHolder> expressions, Expression expression,
			int tuple, Dictionary<Expression, Expression> dictionary)
		{
			_expressions = expressions;
			_parameter = parameter;
			_tuple = tuple;
			_expressions.Add(new ExpressionHolder { Expression = expression, Tuple = tuple }); //ID placeholder
			_dictionary = dictionary;
		}

		public override Expression Visit(Expression expression)
		{
			if (expression == null)
				return null;
			if (_dictionary.TryGetValue(expression, out Expression replacement))
				return replacement;

			return base.Visit(expression);
		}

		protected override Expression VisitMember(MemberExpression expression)
			=> AddAndConvert(expression);

		protected override Expression VisitQuerySourceReference(QuerySourceReferenceExpression expression)
			=> AddAndConvert(expression);

		private Expression AddAndConvert(Expression expression)
		{
			_expressions.Add(new ExpressionHolder { Expression = expression, Tuple = _tuple });

			return Expression.Convert(
				Expression.ArrayIndex(
					Expression.Property(_parameter, Tuple.ItemsProperty),
					Expression.Constant(_expressions.Count - 1)),
				expression.Type);
		}
	}
}