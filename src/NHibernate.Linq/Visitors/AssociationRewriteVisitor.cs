using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Engine;
using NHibernate.Linq.Expressions;
using NHibernate.Linq.Util;
using NHibernate.Metadata;
using NHibernate.Persister.Entity;
using NHibernate.Type;
using IQueryable=System.Linq.IQueryable;

namespace NHibernate.Linq.Visitors
{
	public class AssociationRewriteVisitor : NHibernateExpressionVisitor
	{
		private readonly ISessionFactoryImplementor sessionFactory;
		private int aliasOrder;

		public AssociationRewriteVisitor(ISessionFactoryImplementor factory)
		{
			sessionFactory = factory;
			aliasOrder = 0;
		}

		public static Expression Rewrite(Expression expr, ISessionFactoryImplementor factory)
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
			{
				var type = c.Value.GetType();
				var elementType = ((IQueryable) c.Value).ElementType;
				var loadable = sessionFactory.GetEntityPersister(elementType.FullName) as IOuterJoinLoadable;
				return new QuerySourceExpression(type, loadable);
			}
				
			return c;
		}
	}
}