using System.Linq;
using System.Linq.Expressions;
using NHibernate.Engine;
using NHibernate.Linq.Expressions;
using NHibernate.Linq.Util;

namespace NHibernate.Linq.Visitors
{
	public class NHExpressionToSqlExpressionTransformer : NHibernateExpressionVisitor
	{
		private readonly ISessionFactoryImplementor sessionFactory;

		public NHExpressionToSqlExpressionTransformer(ISessionFactoryImplementor sessionFactory)
		{
			this.sessionFactory = sessionFactory;
		}

		public static Expression Transform(ISessionFactory factory, Expression expr)
		{
			return new NHExpressionToSqlExpressionTransformer(factory as ISessionFactoryImplementor).Visit(expr);
		}

		protected override Expression VisitMethodCall(MethodCallExpression m)
		{
			if (m.Method.DeclaringType == typeof (Enumerable) || m.Method.DeclaringType == typeof (Queryable))
			{
				if (m.Method.Name == "Where")
				{
					return TransformWhereCall(m);
				}
				else if (m.Method.Name == "Select")
				{
					return TransformSelectCall(m);
				}
			}
			return base.VisitMethodCall(m);
		}

		protected Expression TransformSelectCall(MethodCallExpression expr)
		{
			Expression source = Visit(expr.Arguments[0]);
			var lambda = LinqUtil.StripQuotes(expr.Arguments[1]) as LambdaExpression;
			Expression body = lambda.Body;
			ParameterExpression parameter = lambda.Parameters[0];
			string alias = parameter.Name;
			System.Type type = lambda.Body.Type;
			System.Type resultType = typeof (IQueryable<>).MakeGenericType(type);
			return new SelectExpression(resultType, alias, body, source, null);
		}

		protected Expression TransformWhereCall(MethodCallExpression expr)
		{
			Expression source = Visit(expr.Arguments[0]);
			Expression expression = expr.Arguments[1];
			var lambda = LinqUtil.StripQuotes(expr.Arguments[1]) as LambdaExpression;
			Expression body = lambda.Body;
			body = Visit(lambda.Body);
			ParameterExpression parameter = lambda.Parameters[0];
			string alias = parameter.Name;
			System.Type type = body.Type;
			System.Type resultType = typeof (IQueryable<>).MakeGenericType(type);
			return new SelectExpression(resultType, alias, null, source, body);
		}

		protected override Expression VisitQuerySource(QuerySourceExpression expr)
		{
			return expr;
		}


		protected override Expression VisitBinary(BinaryExpression b)
		{
			switch (b.NodeType)
			{
				case ExpressionType.NotEqual: //NOT EQUAL requires special handling since sql servers need expressions like NOT(x=y)
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