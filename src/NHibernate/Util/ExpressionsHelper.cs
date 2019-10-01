using System.Linq.Expressions;
using System.Reflection;
using System;
using NHibernate.Engine;
using NHibernate.Type;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;

namespace NHibernate.Util
{
	public static class ExpressionsHelper
	{
		public static MemberInfo DecodeMemberAccessExpression<TEntity, TResult>(Expression<Func<TEntity, TResult>> expression)
		{
			if (expression.Body.NodeType != ExpressionType.MemberAccess)
			{
				throw new HibernateException(
					string.Format("Invalid expression type: Expected ExpressionType.MemberAccess, Found {0}", expression.Body.NodeType));
			}
			return ((MemberExpression)expression.Body).Member;
		}

		internal static string TryGetEntityName(ISessionFactoryImplementor sessionFactory, MemberExpression memberExpression, out string memberPath)
		{
			string entityName;
			memberPath = memberExpression.Member.Name;
			// When having components we need to go though them in order to find the entity
			while (memberExpression.Expression is MemberExpression subMemberExpression)
			{
				// In some cases we can encounter a property representing the entity e.g. [_0].Customer.CustomerId
				if (subMemberExpression.NodeType == ExpressionType.MemberAccess)
				{
					entityName = sessionFactory.TryGetGuessEntityName(memberExpression.Member.ReflectedType);
					if (entityName != null)
					{
						return entityName;
					}
				}

				memberPath = $"{subMemberExpression.Member.Name}.{memberPath}"; // Build a path that can be used to get the property form the entity metadata
				memberExpression = subMemberExpression;
			}

			// Try to get the actual entity type from the query source if possbile as member can be declared
			// in a base type
			if (memberExpression.Expression is QuerySourceReferenceExpression querySourceReferenceExpression)
			{
				entityName = sessionFactory.TryGetGuessEntityName(querySourceReferenceExpression.Type);
				if (entityName != null ||
					!(querySourceReferenceExpression.ReferencedQuerySource is IFromClause fromClause) ||
					!(fromClause.FromExpression is MemberExpression subMemberExpression))
				{
					return entityName;
				}

				// When the member type is not the one that is mapped (e.g. interface) we have to find the first
				// mapped entity and calculate the entity name from there
				entityName = TryGetEntityName(sessionFactory, subMemberExpression, out var subMemberPath);
				if (entityName == null)
				{
					return null;
				}

				var persister = sessionFactory.GetEntityPersister(entityName);
				var index = persister.EntityMetamodel.GetPropertyIndexOrNull(subMemberPath);
				IAssociationType associationType;
				if (index.HasValue)
				{
					associationType = persister.PropertyTypes[index.Value] as IAssociationType;
				}
				else
				{
					associationType = persister.EntityMetamodel.GetIdentifierPropertyType(subMemberPath) as IAssociationType;
				}

				return associationType?.GetAssociatedEntityName(sessionFactory);
			}

			return sessionFactory.TryGetGuessEntityName(memberExpression.Member.ReflectedType);
		}
	}
}
