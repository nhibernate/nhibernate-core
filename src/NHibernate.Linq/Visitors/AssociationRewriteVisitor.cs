using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Linq.Expressions;
using NHibernate.Linq.Util;
using NHibernate.Metadata;
using NHibernate.Type;

namespace NHibernate.Linq.Visitors
{
	public class AssociationRewriteVisitor : NHibernateExpressionVisitor
	{
		private readonly ISessionFactory sessionFactory;
		private int aliasOrder;

		public AssociationRewriteVisitor(ISessionFactory factory)
		{
			sessionFactory = factory;
			aliasOrder = 0;
		}

		public static Expression Rewrite(Expression expr, ISessionFactory factory)
		{
			var visitor = new AssociationRewriteVisitor(factory);
			expr = visitor.Visit(expr);
			return expr;
		}

		private IClassMetadata GetMetaData(System.Type type)
		{
			if (!LinqUtil.IsAnonymousType(type))
			{
				try
				{
					return sessionFactory.GetClassMetadata(type);
				}
				catch (MappingException)
				{
				}
			}
			return null;
		}

		private string GetNextAlias()
		{
			return "source" + (aliasOrder++);
		}

		protected override Expression VisitMemberAccess(MemberExpression expr)
		{
			expr = (MemberExpression) base.VisitMemberAccess(expr);
			IClassMetadata clazz = GetMetaData(expr.Member.DeclaringType);
			IType propertyType = clazz.GetPropertyType(expr.Member.Name);
			if (propertyType.IsAssociationType || propertyType.IsComponentType)
			{
				return new PropertyExpression(expr.Member.Name, ((PropertyInfo) expr.Member).PropertyType,
				                              base.Visit(expr.Expression), propertyType);
			}
			else
			{
				return new PropertyExpression(expr.Member.Name, ((PropertyInfo) expr.Member).PropertyType,
				                              base.Visit(expr.Expression), propertyType);
			}
		}

		protected override Expression VisitConstant(ConstantExpression c)
		{
			if (c.Value is IQueryable)
				return new QuerySourceExpression((IQueryable) c.Value);
			return c;
		}
	}
}