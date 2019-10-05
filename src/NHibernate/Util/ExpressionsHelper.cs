using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Engine;
using NHibernate.Linq;
using NHibernate.Linq.Expressions;
using NHibernate.Linq.Functions;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
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

		internal static string TryGetEntityName(
			ISessionFactoryImplementor sessionFactory,
			Expression expression,
			out string memberPath,
			out IType memberType)
		{
			var memberPaths = TryGetAllMemberMetadata(sessionFactory, expression, out var entityName, out var convertType);
			if (memberPaths == null)
			{
				memberPath = null;
				memberType = null;
				return null;
			}

			entityName = GetEntityName(entityName, convertType, sessionFactory, out var persister);
			if (entityName == null || memberPaths.Count == 0) // ((NotMapped)q).Prop || q
			{
				memberPath = null;
				memberType = null;
				return entityName;
			}

			var member = memberPaths.Pop();
			var type = persister.EntityMetamodel.GetPropertyType(member.Path);
			while (true)
			{
				if (type == null) // q.NotMappedProp
				{
					memberPath = null;
					memberType = null;
					return entityName;
				}

				if (memberPaths.Count == 0) // q.ManyToOne || q.OneToMany || q.OneToMany[0] || q.Component || q.Prop || q.AnyType
				{
					memberPath = member.Path;
					memberType = GetType(entityName, type, memberPath, member.HasIndexer, sessionFactory);
					return entityName;
				}

				if (type is IAssociationType associationType)
				{
					if (associationType.IsCollectionType) 
					{
						// Check manually for entity association as GetAssociatedEntityName throws when there is none.
						var queryableCollection =
							(IQueryableCollection) associationType.GetAssociatedJoinable(sessionFactory);
						if (!queryableCollection.ElementType.IsEntityType) // q.OneToMany[0].CompositeElement
						{
							entityName = null;
							persister = null;
							// Ignore if the type is casted as composite elements cannot be casted to entities
							type = queryableCollection.ElementType;
							member = memberPaths.Pop();
							continue;
						}

						// q.OneToMany[0].Prop
						entityName = GetEntityName(
							associationType.GetAssociatedEntityName(sessionFactory),
							member.ConvertType,
							sessionFactory,
							out persister);
					}
					else if (associationType.IsAnyType) 
					{
						// ((Address)q.AnyType).Prop || q.AnyType.Prop
						// Unfortunately we cannot detect the exact entity name as cast does not provide it,
						// so the only option is to guess it.
						entityName = GetEntityName(member.ConvertType, sessionFactory, out persister);
					}
					else // q.ManyToOne.Prop
					{
						entityName = GetEntityName(
							associationType.GetAssociatedEntityName(sessionFactory),
							member.ConvertType,
							sessionFactory,
							out persister);
					}

					if (entityName == null) // q.AnyType.Prop || ((NotMappedClass)q.ManyToOne).Prop
					{
						memberPath = null;
						memberType = null;
						return null;
					}

					member = memberPaths.Pop();
					type = persister.EntityMetamodel.GetPropertyType(member.Path);
				}
				else if (type is IAbstractComponentType componentType)
				{
					// q.OneToMany[0].CompositeElement.Prop
					if (entityName == null)
					{
						var index = GetComponentPropertyIndex(componentType, member.Path);
						if (!index.HasValue)
						{
							memberPath = null;
							memberType = null;
							return null;
						}

						type = componentType.Subtypes[index.Value];
						continue;
					}

					// q.Component.Prop
					// Ignore if the type is casted as components cannot be casted to entities
					var componentMember = memberPaths.Pop();
					member = new MemberMetadata(
						$"{member.Path}.{componentMember.Path}",
						componentMember.ConvertType,
						componentMember.HasIndexer);
					type = persister.EntityMetamodel.GetPropertyType(member.Path);
				}
				else
				{
					// q.Prop.NotMappedProp
					memberPath = null;
					memberType = null;
					return entityName;
				}
			}
		}

		private static Stack<MemberMetadata> TryGetAllMemberMetadata(
			ISessionFactoryImplementor sessionFactory,
			Expression expression,
			out string entityName,
			out System.Type convertType)
		{
			var memberPaths = new Stack<MemberMetadata>();
			var currentExpression = expression;
			convertType = null;
			bool hasIndexer = false;
			while (true)
			{
				if (currentExpression is MemberExpression subMemberExpression)
				{
					memberPaths.Push(new MemberMetadata(subMemberExpression.Member.Name, convertType, hasIndexer));
					convertType = null;
					hasIndexer = false;
					currentExpression = subMemberExpression.Expression;
				}
				else if (currentExpression is QuerySourceReferenceExpression querySourceReferenceExpression)
				{
					if (querySourceReferenceExpression.ReferencedQuerySource is IFromClause fromClause)
					{
						currentExpression = fromClause.FromExpression;
					}
					else if (querySourceReferenceExpression.ReferencedQuerySource is JoinClause joinClause)
					{
						currentExpression = joinClause.InnerSequence;
					}
					else
					{
						// Unknown ReferencedQuerySource
						entityName = null;
						return null;
					}
				}
				else if (currentExpression is UnaryExpression unaryExpression) // ((BaseEntity)q.Entity).Prop
				{
					currentExpression = unaryExpression.Operand;
					convertType = unaryExpression.Type;
				}
				else if (currentExpression is NhNominatedExpression nominatedExpression) // ((BaseEntity)q.Entity).Prop
				{
					currentExpression = nominatedExpression.Expression;
				}
				else if (currentExpression is ConstantExpression constantExpression)
				{
					if (!(constantExpression.Value is IEntityNameProvider entityNameProvider))
					{
						// Not a NhQueryable<T>
						entityName = null;
						return null;
					}

					entityName = entityNameProvider.EntityName;
					break;
				}
				else if (currentExpression is MethodCallExpression methodCallExpression &&
						ListIndexerGenerator.IsMethodSupported(methodCallExpression.Method))
				{
					currentExpression = methodCallExpression.Object == null
						? Enumerable.First(methodCallExpression.Arguments) // q.Children.ElementAt(0)
						: methodCallExpression.Object; // q.Children[0]
					hasIndexer = true;
				}
				else
				{
					// Not supported expressions
					entityName = null;
					return null;
				}
			}

			return memberPaths;
		}

		private static string GetEntityName(
			string currentEntityName,
			System.Type convertedType,
			ISessionFactoryImplementor sessionFactory,
			out IEntityPersister persister)
		{
			persister = sessionFactory.TryGetEntityPersister(currentEntityName);
			if (persister == null)
			{
				return null; // Querying an unmapped interface e.g. s.Query<IEntity>().Where(a => a.Type == "A")
			}

			if (convertedType == null)
			{
				return currentEntityName;
			}

			if (persister.EntityMetamodel.HasSubclasses)
			{
				// When a class is casted to a subclass e.g. ((PizzaOrder)c.Order).PizzaName, we
				// can only guess the entity name of it, as there can be many entity names mapped
				// to the same subclass.
				persister = persister.EntityMetamodel.SubclassEntityNames
									.Select(sessionFactory.GetEntityPersister)
									.FirstOrDefault(p => p.MappedClass == convertedType);

				return persister?.EntityName;
			}

			return GetEntityName(convertedType, sessionFactory, out persister);
		}

		private static string GetEntityName(
			System.Type convertedType,
			ISessionFactoryImplementor sessionFactory,
			out IEntityPersister persister)
		{
			if (convertedType == null)
			{
				persister = null;
				return null;
			}

			var entityName = sessionFactory.TryGetGuessEntityName(convertedType);
			if (entityName == null)
			{
				persister = null;
				return null;
			}

			persister = sessionFactory.GetEntityPersister(entityName);
			return entityName;
		}

		private static int? GetComponentPropertyIndex(IAbstractComponentType componentType, string name)
		{
			var names = componentType.PropertyNames;
			for (var i = 0; i < names.Length; i++)
			{
				if (names[i].Equals(name))
				{
					return i;
				}
			}

			return null;
		}

		private static IType GetType(
			string entityName,
			IType currentType,
			string memberPath,
			bool hasIndexer,
			ISessionFactoryImplementor sessionFactory)
		{
			if (entityName == null && memberPath == null)
			{
				return null;
			}

			if (entityName == null)
			{
				// q.OneToMany[0].CompositeElement.Prop
				if (currentType is IAbstractComponentType componentType)
				{
					var index = GetComponentPropertyIndex(componentType, memberPath);
					return index.HasValue
						? componentType.Subtypes[index.Value]
						: null;
				}

				return null;
			}

			if (memberPath == null) // q.NotMappedProp
			{
				return null;
			}

			if (!hasIndexer) // q.Prop
			{
				return currentType;
			}

			// q.OneToMany[0]
			if (currentType is IAssociationType associationType)
			{
				var queryableCollection =
					(IQueryableCollection) associationType.GetAssociatedJoinable(sessionFactory);
				return queryableCollection.ElementType;
			}

			// q.Prop[0]
			return null;
		}

		private struct MemberMetadata
		{
			public MemberMetadata(string path, System.Type convertType, bool hasIndexer)
			{
				Path = path;
				ConvertType = convertType;
				HasIndexer = hasIndexer;
			}

			public string Path { get; }

			public System.Type ConvertType { get; }

			public bool HasIndexer { get; }
		}
	}
}
