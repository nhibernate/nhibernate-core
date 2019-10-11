using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Engine;
using NHibernate.Linq;
using NHibernate.Linq.Expressions;
using NHibernate.Linq.Functions;
using NHibernate.Linq.Visitors;
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

		/// <summary>
		/// Try to get the mapped nullability from the given expression.
		/// </summary>
		/// <param name="sessionFactory">The session factory.</param>
		/// <param name="expression">The expression to evaluate.</param>
		/// <param name="nullable">Output parameter that represents whether the <paramref name="expression"/> is nullable.</param>
		/// <returns>Whether the mapped nullability was found.</returns>
		internal static bool TryGetMappedNullability(
			ISessionFactoryImplementor sessionFactory,
			Expression expression,
			out bool nullable)
		{
			if (!TryGetMappedType(
				sessionFactory,
				expression,
				out _,
				out var entityPersister,
				out var componentType,
				out var memberPath))
			{
				nullable = false;
				return false;
			}

			// The source entity is always not null, as it gets translated to the entity identifier
			if (memberPath == null)
			{
				nullable = false;
				return true;
			}

			int index;
			if (componentType != null)
			{
				index = Array.IndexOf(
					componentType.PropertyNames,
					memberPath.Substring(memberPath.LastIndexOf('.') + 1));
				nullable = componentType.PropertyNullability[index];
				return true;
			}

			if (entityPersister.EntityMetamodel.GetIdentifierPropertyType(memberPath) != null)
			{
				nullable = false; // Identifier is always not-null
				return true;
			}

			index = entityPersister.EntityMetamodel.GetPropertyIndex(memberPath);
			nullable = entityPersister.PropertyNullability[index];
			return true;
		}

		/// <summary>
		/// Try to get the mapped type from the given expression. When the <paramref name="expression"/> type is
		/// <see cref="ExpressionType.Convert"/>, the <paramref name="mappedType"/> will be set based on the expression type
		/// only when the mapping for <see cref="UnaryExpression.Operand"/> was found, otherwise <see langword="null"/>
		/// will be returned.
		/// </summary>
		/// <param name="sessionFactory">The session factory to retrieve <see cref="IEntityPersister"/> types.</param>
		/// <param name="expression">The expression to evaluate.</param>
		/// <param name="mappedType">Output parameter that represents the mapped type of <paramref name="memberPath"/>.</param>
		/// <param name="entityPersister">
		/// Output parameter that represents the entity persister of the entity where <paramref name="memberPath"/> is defined.
		/// This parameter will not be set when <paramref name="memberPath"/> represents a property in a collection composite element.
		/// </param>
		/// <param name="component">
		/// Output parameter that represents the component type where <paramref name="memberPath"/> is defined.
		/// This parameter will not be set when <paramref name="memberPath"/> does not represent a property in a component.
		/// </param>
		/// <param name="memberPath">
		/// Output parameter that represents the path of the mapped member, which in most cases is the member name. In case
		/// when the mapped member is defined inside a component the path will be prefixed with the name of the component member and a dot.
		/// (e.g. Component.Property).</param>
		/// <returns>Whether the mapped type was found.</returns>
		/// <remarks>
		/// When the <paramref name="expression"/> contains an expression of type <see cref="ExpressionType.Convert"/>, the
		/// result may not be correct when casting to an entity that is mapped with multiple entity names.
		/// </remarks>
		internal static bool TryGetMappedType(
			ISessionFactoryImplementor sessionFactory,
			Expression expression,
			out IType mappedType,
			out IEntityPersister entityPersister,
			out IAbstractComponentType component,
			out string memberPath)
		{
			// In order to get the correct entity name from the expression we first have to find the constant expression that contains the
			// IEntityNameProvider instance, from which we can retrive the starting entity name. Once we have it, we have to traverse all
			// expressions that we had to travese in order to find the IEntityNameProvider instance, but in reverse order (bottom to top)
			// and keep tracking the entity name until we reach to top.

			// Try to retrive the starting entity name with all members that were traversed in that process
			var memberPaths = MemberMetadataExtractor.TryGetAllMemberMetadata(expression, out var entityName, out var convertType);
			if (memberPaths == null || !TryGetEntityPersister(entityName, null, sessionFactory, out var currentEntityPersister))
			{
				// Failed to find the starting entity name, due to:
				// - Unsupported expression
				// - The expression didn't contain the IEntityNameProvider instance
				// - Querying an unmapped type e.g. s.Query<IEntity>().Where(a => a.Type == "A")
				memberPath = null;
				mappedType = null;
				entityPersister = null;
				component = null;
				return false;
			}

			if (memberPaths.Count == 0) // The expression do not contain any member expressions
			{
				if (convertType != null)
				{
					mappedType = TryGetEntityPersister(currentEntityPersister, convertType, sessionFactory, out var convertPersister)
						? convertPersister.EntityMetamodel.EntityType // ((Subclass)q)
						: TypeFactory.GetDefaultTypeFor(convertType); // ((NotMapped)q)
				}
				else
				{
					mappedType = currentEntityPersister.EntityMetamodel.EntityType; // q
				}

				memberPath = null;
				component = null;
				entityPersister = currentEntityPersister;
				return mappedType != null;
			}

			// If there was a cast right after the constant expression that contains the IEntityNameProvider instance, we have
			// to update the entity persister according to it, otherwise use the value returned by TryGetAllMemberMetadata method.
			if (convertType != null)
			{
				if (!TryGetEntityPersister(currentEntityPersister, convertType, sessionFactory, out var convertPersister)) // ((NotMapped)q).Id
				{
					memberPath = null;
					mappedType = null;
					entityPersister = null;
					component = null;
					return false;
				}
				else
				{
					currentEntityPersister = convertPersister; // ((Subclass)q).Id
				}
			}

			return TraverseMembers(
				sessionFactory,
				memberPaths,
				currentEntityPersister,
				out mappedType,
				out entityPersister,
				out component,
				out memberPath);
		}

		private static bool TraverseMembers(
			ISessionFactoryImplementor sessionFactory,
			Stack<MemberMetadata> memberPaths,
			IEntityPersister currentEntityPersister,
			out IType mappedType,
			out IEntityPersister entityPersister,
			out IAbstractComponentType component,
			out string memberPath)
		{
			// Traverse the members that were traversed by the TryGetAllMemberMetadata method in the reverse order and try to keep
			// tracking the entity persister until all members are traversed.
			var member = memberPaths.Pop();
			var currentType = currentEntityPersister.EntityMetamodel.GetPropertyType(member.Path);
			IAbstractComponentType currentComponentType = null;
			while (memberPaths.Count > 0 && currentType != null)
			{
				var convertType = member.ConvertType;

				switch (currentType)
				{
					case IAssociationType associationType:
						member = memberPaths.Pop();
						ProcessAssociationType(
							associationType,
							sessionFactory,
							member,
							convertType,
							out currentType,
							out currentEntityPersister,
							out currentComponentType);
						break;
					case IAbstractComponentType componentType:
						// Concatenate the component property path in order to be able to use EntityMetamodel.GetPropertyType to retrieve the type.
						// As GetPropertyType supports only components, do not concatenate when dealing with collection composite elements or elements.
						if (!currentType.IsAnyType)
						{
							var nextMember = memberPaths.Pop();
							member = currentEntityPersister == null // Collection with composite element or element
								? nextMember
								: new MemberMetadata($"{member.Path}.{nextMember.Path}", nextMember.ConvertType, nextMember.HasIndexer);
						}
						else
						{
							member = memberPaths.Pop();
						}
						currentComponentType = componentType;
						ProcessComponentType(componentType, currentEntityPersister, member, out currentType);
						break;
					default:
						member = memberPaths.Pop();
						// q.Prop.NotMappedProp
						currentType = null;
						currentEntityPersister = null;
						currentComponentType = null;
						break;
				}
			}
			
			// When traversed to the top of the expression, return the current tracking values
			if (memberPaths.Count == 0)
			{
				memberPath = currentEntityPersister != null || currentComponentType != null ? member.Path : null;
				mappedType = GetType(currentEntityPersister, currentType, member, sessionFactory, out _);
				entityPersister = currentEntityPersister;
				component = currentComponentType;
				return mappedType != null;
			}

			// Member not mapped
			memberPath = null;
			mappedType = null;
			entityPersister = null;
			component = null;
			return false;
		}

		private static void ProcessComponentType(
			IAbstractComponentType componentType,
			IEntityPersister persister,
			MemberMetadata member,
			out IType memberType)
		{
			// When persister is not available (q.OneToManyCompositeElement[0].Prop), try to get the type from the component
			if (persister == null)
			{
				var index = Array.IndexOf(componentType.PropertyNames, member.Path);
				memberType = index < 0
					? null // q.OneToManyCompositeElement[0].NotMappedProp
					: componentType.Subtypes[index]; // q.OneToManyCompositeElement[0].Prop
				return;
			}

			// q.Component.Prop
			memberType = persister.EntityMetamodel.GetPropertyType(member.Path);
		}

		private static void ProcessAssociationType(
			IAssociationType associationType,
			ISessionFactoryImplementor sessionFactory,
			MemberMetadata member,
			System.Type convertType,
			out IType memberType,
			out IEntityPersister memberPersister,
			out IAbstractComponentType memberComponent)
		{
			if (associationType.IsCollectionType)
			{
				// Check manually for entity association as GetAssociatedEntityName throws when there is none.
				var queryableCollection =
					(IQueryableCollection) associationType.GetAssociatedJoinable(sessionFactory);
				if (!queryableCollection.ElementType.IsEntityType) // q.OneToManyCompositeElement[0].Member, q.OneToManyElement[0].Member
				{
					memberPersister = null;
					// Can be <composite-element> or <element>
					switch (queryableCollection.ElementType)
					{
						case IAbstractComponentType componentType: // q.OneToManyCompositeElement[0].Member
							memberComponent = componentType;
							ProcessComponentType(componentType, null, member, out memberType);
							return;
						default: // q.OneToManyElement[0].Member
							memberType = null;
							memberComponent = null;
							return;
					}
				}

				// q.OneToMany[0].Member
				TryGetEntityPersister(
					associationType.GetAssociatedEntityName(sessionFactory),
					convertType,
					sessionFactory,
					out memberPersister);
			}
			else if (associationType.IsAnyType)
			{
				// ((Address)q.AnyType).Member, q.AnyType.Member
				// Unfortunately we cannot detect the exact entity name as cast does not provide it,
				// so the only option is to guess it.
				TryGetEntityPersister(convertType, sessionFactory, out memberPersister);
			}
			else // q.ManyToOne.Member
			{
				TryGetEntityPersister(
					associationType.GetAssociatedEntityName(sessionFactory),
					convertType,
					sessionFactory,
					out memberPersister);
			}

			memberComponent = null;
			memberType = memberPersister != null
				? memberPersister.EntityMetamodel.GetPropertyType(member.Path)
				: null; // q.AnyType.Member, ((NotMappedClass)q.ManyToOne)
		}

		private static bool TryGetEntityPersister(
			string currentEntityName,
			System.Type convertedType,
			ISessionFactoryImplementor sessionFactory,
			out IEntityPersister persister)
		{
			var currentEntityPersister = sessionFactory.TryGetEntityPersister(currentEntityName);
			if (currentEntityPersister == null)
			{
				persister = null;
				return false; // Querying an unmapped interface e.g. s.Query<IEntity>().Where(a => a.Type == "A")
			}

			return TryGetEntityPersister(currentEntityPersister, convertedType, sessionFactory, out persister);
		}

		private static bool TryGetEntityPersister(
			IEntityPersister currentEntityPersister,
			System.Type convertedType,
			ISessionFactoryImplementor sessionFactory,
			out IEntityPersister persister)
		{
			if (convertedType == null)
			{
				persister = currentEntityPersister;
				return true;
			}

			if (currentEntityPersister.EntityMetamodel.HasSubclasses)
			{
				// When a class is casted to a subclass e.g. ((PizzaOrder)c.Order).PizzaName, we
				// can only guess the entity name of it, as there can be many entity names mapped
				// to the same subclass.
				persister = currentEntityPersister.EntityMetamodel.SubclassEntityNames
									.Select(sessionFactory.GetEntityPersister)
									.FirstOrDefault(p => p.MappedClass == convertedType);

				return persister != null;
			}

			return TryGetEntityPersister(convertedType, sessionFactory, out persister);
		}

		private static bool TryGetEntityPersister(
			System.Type convertedType,
			ISessionFactoryImplementor sessionFactory,
			out IEntityPersister persister)
		{
			if (convertedType == null)
			{
				persister = null;
				return false;
			}

			var entityName = sessionFactory.TryGetGuessEntityName(convertedType);
			if (entityName == null)
			{
				persister = null;
				return false;
			}

			persister = sessionFactory.GetEntityPersister(entityName);
			return true;
		}

		private static IType GetType(
			IEntityPersister currentEntityPersister,
			IType currentType,
			MemberMetadata member,
			ISessionFactoryImplementor sessionFactory,
			out IEntityPersister persister)
		{
			// Not mapped
			if (currentType == null)
			{
				persister = null;
				return null;
			}

			// Collection composite elements
			if (currentEntityPersister == null)
			{
				if (member.ConvertType != null)
				{
					return TryGetEntityPersister(member.ConvertType, sessionFactory, out persister)
						? persister.EntityMetamodel.EntityType // (Entity)q.OneToManyCompositeElement[0].Prop
						: TypeFactory.GetDefaultTypeFor(member.ConvertType); // (long)q.OneToManyCompositeElement[0].Prop
				}

				persister = null;
				return currentType;
			}

			if (!member.HasIndexer)
			{
				if (member.ConvertType != null)
				{
					persister = TryGetEntityPersister(member.ConvertType, sessionFactory, out var newPersister)
						? newPersister
						: currentEntityPersister;
					return newPersister != null
						? persister.EntityMetamodel.EntityType // (Entity)q.Prop
						: TypeFactory.GetDefaultTypeFor(member.ConvertType); // (long)q.Prop
				}

				persister = currentEntityPersister;
				return currentType; // q.Prop
			}

			// q.OneToMany[0]
			if (currentType is IAssociationType associationType)
			{
				var queryableCollection = (IQueryableCollection) associationType.GetAssociatedJoinable(sessionFactory);
				if (member.ConvertType != null)
				{
					return TryGetEntityPersister(member.ConvertType, sessionFactory, out persister)
						? persister.EntityMetamodel.EntityType // (Entity)q.Prop
						: TypeFactory.GetDefaultTypeFor(member.ConvertType); // (long)q.Prop
				}

				persister = queryableCollection.ElementType.IsEntityType
					? queryableCollection.ElementPersister
					: null;

				return queryableCollection.ElementType;
			}

			// q.Prop[0]
			persister = null;
			return null;
		}

		private class MemberMetadataExtractor : NhExpressionVisitor
		{
			private readonly Stack<MemberMetadata> _memberPaths = new Stack<MemberMetadata>();
			private System.Type _convertType;
			private bool _hasIndexer;
			private string _entityName;

			/// <summary>
			/// Traverses the expression from top to bottom until the first <see cref="ConstantExpression"/> containing an IEntityNameProvider instance is found.
			/// </summary>
			/// <param name="expression">The expression to travese.</param>
			/// <param name="entityName">An output parameter that will be populated by the first <see cref="IEntityNameProvider.EntityName"/> that is found or null otherwise.</param>
			/// <param name="convertType">An output parameter that will be populated only when <see cref="ConstantExpression"/> containing an IEntityNameProvider 
			/// is followed by an <see cref="UnaryExpression"/>.</param>
			/// <returns>A stack of information about all <see cref="MemberExpression"/> that were traversed until the first <see cref="ConstantExpression"/> containing an 
			/// IEntityNameProvider instance is found or null when it was not found or if one of the expressions is not supported.</returns>
			public static Stack<MemberMetadata> TryGetAllMemberMetadata(
				Expression expression,
				out string entityName,
				out System.Type convertType)
			{
				var extractor = new MemberMetadataExtractor();
				extractor.Accept(expression);
				entityName = extractor._entityName;
				convertType = entityName != null ? extractor._convertType : null;
				return entityName != null ? extractor._memberPaths : null;
			}

			private void Accept(Expression expression)
			{
				base.Visit(expression);
			}

			protected override Expression VisitMember(MemberExpression node)
			{
				_memberPaths.Push(new MemberMetadata(node.Member.Name, _convertType, _hasIndexer));
				_convertType = null;
				_hasIndexer = false;
				return base.Visit(node.Expression);
			}

			protected override Expression VisitQuerySourceReference(QuerySourceReferenceExpression node)
			{
				if (node.ReferencedQuerySource is IFromClause fromClause)
				{
					return base.Visit(fromClause.FromExpression);
				}

				if (node.ReferencedQuerySource is JoinClause joinClause)
				{
					return base.Visit(joinClause.InnerSequence);
				}

				// Not supported expression
				_entityName = null;
				return node;
			}

			protected override Expression VisitUnary(UnaryExpression node)
			{
				// Store only the outermost cast, when there are multiple casts for the same member
				if (_convertType == null)
				{
					_convertType = node.Type;
				}

				return base.Visit(node.Operand);
			}

			protected internal override Expression VisitNhNominated(NhNominatedExpression node)
			{
				return base.Visit(node.Expression);
			}

			protected override Expression VisitConstant(ConstantExpression node)
			{
				_entityName = node.Value is IEntityNameProvider entityNameProvider
					? entityNameProvider.EntityName
					: null; // Not a NhQueryable<T>

				return node;
			}

			protected override Expression VisitBinary(BinaryExpression node)
			{
				if (node.NodeType == ExpressionType.ArrayIndex)
				{
					_hasIndexer = true;
					return base.Visit(node.Left);
				}

				return base.VisitBinary(node);
			}

			protected override Expression VisitMethodCall(MethodCallExpression node)
			{
				if (ListIndexerGenerator.IsMethodSupported(node.Method))
				{
					_hasIndexer = true;
					return base.Visit(
						node.Object == null
							? Enumerable.First(node.Arguments) // q.Children.ElementAt(0)
							: node.Object // q.Children[0]
					);
				}

				// Not supported expression
				_entityName = null;
				return node;
			}

			public override Expression Visit(Expression node)
			{
				// Not supported expression
				_entityName = null;
				return node;
			}
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
