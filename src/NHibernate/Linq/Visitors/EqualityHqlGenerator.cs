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
		    return innerKeySelector is NewExpression && outerKeySelector is NewExpression
		               ? VisitNew((NewExpression) innerKeySelector, (NewExpression) outerKeySelector)
		               : GenerateEqualityNode(innerKeySelector, outerKeySelector);
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
			return GenerateEqualityNode(innerKeySelector.Arguments[index], outerKeySelector.Arguments[index]);
		}

		private HqlEquality GenerateEqualityNode(Expression leftExpr, Expression rightExpr)
		{
            // TODO - why two visitors? Can't we just reuse?
			var left = new HqlGeneratorExpressionTreeVisitor(_parameters);
			var right = new HqlGeneratorExpressionTreeVisitor(_parameters);

		    return _hqlTreeBuilder.Equality(left.Visit(leftExpr).AsExpression(), right.Visit(rightExpr).AsExpression());
		}
	}
}