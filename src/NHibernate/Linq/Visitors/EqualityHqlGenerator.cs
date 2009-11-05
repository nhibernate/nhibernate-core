using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Engine.Query;
using NHibernate.Hql.Ast;

namespace NHibernate.Linq.Visitors
{
	/// <summary>
	/// Builds HQL Equality nodes and used in joins
	/// </summary>
	public class EqualityHqlGenerator 
	{
		private readonly HqlTreeBuilder _hqlTreeBuilder;
		private readonly IDictionary<ConstantExpression, NamedParameter> _parameters;
		private readonly IList<NamedParameterDescriptor> _requiredHqlParameters;

		public EqualityHqlGenerator(IDictionary<ConstantExpression, NamedParameter> parameters, IList<NamedParameterDescriptor> requiredHqlParameters)
		{
			_parameters = parameters;
			_requiredHqlParameters = requiredHqlParameters;
			_hqlTreeBuilder = new HqlTreeBuilder();
		}

		public HqlTreeNode Visit(Expression innerKeySelector, Expression outerKeySelector)
		{
			if (innerKeySelector is NewExpression && outerKeySelector is NewExpression)
			{
				return VisitNew((NewExpression)innerKeySelector, (NewExpression)outerKeySelector);
			}
			else
			{
				return GenerateEqualityNode(innerKeySelector, outerKeySelector);
			}
		}

		private HqlTreeNode VisitNew(NewExpression innerKeySelector, NewExpression outerKeySelector)
		{
			if (innerKeySelector.Arguments.Count != outerKeySelector.Arguments.Count)
			{
				throw new NotSupportedException();
			}

			HqlTreeNode hql = GenerateEqualityNode(innerKeySelector, outerKeySelector, 0);

			for (int i = 1; i < innerKeySelector.Arguments.Count; i++)
			{
				hql = _hqlTreeBuilder.And(hql, GenerateEqualityNode(innerKeySelector, outerKeySelector, i));
			}

			return hql;
		}

		private HqlEquality GenerateEqualityNode(NewExpression innerKeySelector, NewExpression outerKeySelector, int index)
		{
			return GenerateEqualityNode(innerKeySelector.Arguments[index], outerKeySelector.Arguments[index]);
		}

		private HqlEquality GenerateEqualityNode(Expression leftExpr, Expression rightExpr)
		{
			var left = new HqlGeneratorExpressionTreeVisitor(_parameters, _requiredHqlParameters);
			left.Visit(leftExpr);

			var right = new HqlGeneratorExpressionTreeVisitor(_parameters, _requiredHqlParameters);
			right.Visit(rightExpr);

			return _hqlTreeBuilder.Equality(left.GetHqlTreeNodes().Single(), right.GetHqlTreeNodes().Single());
		}
	}
}