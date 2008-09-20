using System;
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

		protected override Expression VisitMemberAccess(MemberExpression expr)
		{
			expr = (MemberExpression) base.VisitMemberAccess(expr);
			System.Type type = TypeSystem.GetElementType(expr.Type);
			IClassMetadata clazz = GetMetaData(expr.Member.DeclaringType);
			IPropertyMapping mapping = sessionFactory.GetEntityPersister(expr.Type.FullName) as IPropertyMapping;
			IType propertyType = clazz.GetPropertyType(expr.Member.Name);
			string propertyName = expr.Member.Name;
			if (propertyType.IsComponentType)
			{
				Expression source = base.Visit(expr.Expression);
				return new ComponentPropertyExpression(propertyName, mapping.ToColumns(propertyName),
				                                       ((PropertyInfo) expr.Member).PropertyType,
				                                       source, propertyType);
			}
			else if (propertyType is OneToOneType)
			{
				Expression source = base.Visit(expr.Expression);
				return new OneToOnePropertyExpression(propertyName, expr.Type, source, propertyType);
			}
			else if(!propertyType.IsAssociationType)//assume simple property
			{
				return new SimplePropertyExpression(expr.Member.Name, mapping.ToColumns(propertyName)[0],
				                                    ((PropertyInfo) expr.Member).PropertyType,
				                                    base.Visit(expr.Expression), propertyType);
			}
			return expr;
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