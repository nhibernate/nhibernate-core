using System;
using System.Linq.Expressions;
using NHibernate.Hql.Ast;

namespace NHibernate.Linq.Visitors
{
	/// <summary>
	/// Builds HQL Equality nodes and used in joins
	/// </summary>
	public class EqualityHqlGenerator 
	{
		private readonly HqlTreeBuilder _hqlTreeBuilder;
		private readonly VisitorParameters _parameters;

		public EqualityHqlGenerator(VisitorParameters parameters)
		{
			_parameters = parameters;
			_hqlTreeBuilder = new HqlTreeBuilder();
		}

		public HqlBooleanExpression Visit(Expression innerKeySelector, Expression outerKeySelector)
		{
			var innerNewExpression = innerKeySelector as NewExpression;
			var outerNewExpression = outerKeySelector as NewExpression;
			return innerNewExpression != null && outerNewExpression != null
				? VisitNew(innerNewExpression, outerNewExpression)
				: GenerateEqualityNode(innerKeySelector, outerKeySelector, new HqlGeneratorExpressionTreeVisitor(_parameters));
		}

		private HqlBooleanExpression VisitNew(NewExpression innerKeySelector, NewExpression outerKeySelector)
		{
			if (innerKeySelector.Arguments.Count != outerKeySelector.Arguments.Count)
			{
				throw new NotSupportedException();
			}

			HqlBooleanExpression hql = GenerateEqualityNode(innerKeySelector, outerKeySelector, 0);

			for (int i = 1; i < innerKeySelector.Arguments.Count; i++)
			{
				hql = _hqlTreeBuilder.BooleanAnd(hql, GenerateEqualityNode(innerKeySelector, outerKeySelector, i));
			}

			return hql;
		}

		private HqlEquality GenerateEqualityNode(NewExpression innerKeySelector, NewExpression outerKeySelector, int index)
		{
			return GenerateEqualityNode(innerKeySelector.Arguments[index], outerKeySelector.Arguments[index], new HqlGeneratorExpressionTreeVisitor(_parameters));
		}

		private HqlEquality GenerateEqualityNode(Expression leftExpr, Expression rightExpr, IHqlExpressionVisitor visitor)
		{
			return _hqlTreeBuilder.Equality(
				visitor.Visit(leftExpr).ToArithmeticExpression(),
				visitor.Visit(rightExpr).ToArithmeticExpression());
		}
	}
}