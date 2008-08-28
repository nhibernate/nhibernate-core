using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using NHibernate.Impl;
using NHibernate.Linq.Expressions;
using NHibernate.Linq.Util;
using NHibernate.Metadata;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Mapping;
using NHibernate.Engine;
using NHibernate.Persister.Entity;
using IQueryable=System.Linq.IQueryable;

namespace NHibernate.Linq.Visitors
{
	public class NHExpressionToSqlExpressionTransformer : NHibernateExpressionVisitor
	{
		public static Expression Transform(ISessionFactory factory,Expression expr)
		{
			return new NHExpressionToSqlExpressionTransformer(factory as ISessionFactoryImplementor).Visit(expr);
		}

		public NHExpressionToSqlExpressionTransformer(ISessionFactoryImplementor sessionFactory)
		{
			this.sessionFactory = sessionFactory;
		}

		private readonly ISessionFactoryImplementor sessionFactory;

		protected override Expression VisitMethodCall(MethodCallExpression m)
		{
			if(m.Method.DeclaringType==typeof(Enumerable)||m.Method.DeclaringType==typeof(Queryable))
			{
				if(m.Method.Name=="Where")
				{
					return TransformWhereCall(m);
				}
				else if(m.Method.Name=="Select")
				{
					return TransformSelectCall(m);
				}
			}
			return base.VisitMethodCall(m);
		}
		protected Expression TransformSelectCall(MethodCallExpression expr)
		{
			Expression source = this.Visit(expr.Arguments[0]);
			LambdaExpression lambda = LinqUtil.StripQuotes(expr.Arguments[1]) as LambdaExpression;
			Expression body = lambda.Body;
			ParameterExpression parameter = lambda.Parameters[0];
			string alias = parameter.Name;
			System.Type type = lambda.Body.Type;
			System.Type resultType = typeof(IQueryable<>).MakeGenericType(type);
			return new SelectExpression(resultType, alias, body, source, null);
		}

		protected Expression TransformWhereCall(MethodCallExpression expr)
		{
			Expression source = this.Visit(expr.Arguments[0]);
			Expression expression = expr.Arguments[1];
			LambdaExpression lambda = LinqUtil.StripQuotes(expr.Arguments[1]) as LambdaExpression;
			Expression body = lambda.Body;
			body=this.Visit(lambda.Body);
			ParameterExpression parameter = lambda.Parameters[0];
			string alias = parameter.Name;
			System.Type type = body.Type;
			System.Type resultType = typeof(IQueryable<>).MakeGenericType(type);
			return new SelectExpression(resultType, alias, null, source, body);
		}

		protected override Expression VisitQuerySource(QuerySourceExpression expr)
		{
			return expr;
		}

		
		protected override Expression VisitBinary(BinaryExpression b)
		{
			switch(b.NodeType)
			{
				case ExpressionType.NotEqual://NOT EQUAL requires special handling since sql servers need expressions like NOT(x=y)
					{
						Expression eq = Expression.Equal(b.Left, b.Right);
						return Expression.Not(eq);
					}
				default:
					return base.VisitBinary(b);
			}
			
		}
	}
}
