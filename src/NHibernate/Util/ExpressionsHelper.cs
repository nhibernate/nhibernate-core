using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using NHibernate.Engine;
using NHibernate.Linq;
using NHibernate.Linq.Clauses;
using NHibernate.Linq.Expressions;
using NHibernate.Linq.Functions;
using NHibernate.Linq.Visitors;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.Type;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;

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

#if NETCOREAPP2_0
		/// <summary>
		/// Try to retrieve <see cref="GetMemberBinder"/> from a reduced <see cref="ExpressionType.Dynamic"/> expression.
		/// </summary>
		/// <param name="expression">The reduced dynamic expression.</param>
		/// <param name="memberBinder">The out binder parameter.</param>
		/// <returns>Whether the binder was found.</returns>
		internal static bool TryGetDynamicMemberBinder(InvocationExpression expression, out GetMemberBinder memberBinder)
		{
			// This is an ugly workaround for dynamic expressions in .NET Core. In .NET Core a dynamic expression is reduced
			// when first visited by a expression visitor that is not a DynamicExpressionVisitor, where in .NET Framework it is never reduced.
			// As RelinqExpressionVisitor does not extend DynamicExpressionVisitor, we will always have a reduced dynamic expression in .NET Core.
			// Unfortunately we can not tap into the expression tree earlier to intercept the dynamic expression
			if (expression.Arguments.Count == 2 &&
			    expression.Arguments[0] is ConstantExpression constant &&
			    constant.Value is CallSite site &&
			    site.Binder is GetMemberBinder binder)
			{
				memberBinder = binder;
				return true;
			}

			memberBinder = null;
			return false;
		}
#endif

		/// <summary>
		/// Unwraps <see cref="UnaryExpression"/>.
		/// </summary>
		/// <param name="expression">The expression to unwrap.</param>
		/// <returns>The unwrapped expression.</returns>
		internal static Expression UnwrapUnary(Expression expression)
		{
			if (expression is UnaryExpression unaryExpression)
			{
				return UnwrapUnary(unaryExpression.Operand);
			}

			return expression;
		}

		/// <summary>
		/// Check whether the given expression represent a variable.
		/// </summary>
		/// <param name="expression">The expression to check.</param>
		/// <param name="path">The path of the variable.</param>
		/// <param name="closureContext">The closure context where the variable is stored.</param>
		/// <returns>Whether the expression represents a variable.</returns>
		internal static bool IsVariable(Expression expression, out string path, out object closureContext)
		{
			Expression childExpression;
			string currentPath;
			switch (expression)
			{
				case MemberExpression memberExpression:
					childExpression = memberExpression.Expression;
					currentPath = memberExpression.Member.Name;
					break;
				case ConstantExpression constantExpression:
					path = null;
					if (constantExpression.Type.Attributes.HasFlag(TypeAttributes.NestedPrivate) &&
					    Attribute.IsDefined(constantExpression.Type, typeof(CompilerGeneratedAttribute), inherit: true))
					{
						closureContext = constantExpression.Value;
						return true;
					}

					closureContext = null;
					return false;
				default:
					path = null;
					closureContext = null;
					return false;
			}

			if (!IsVariable(childExpression, out path, out closureContext))
			{
				return false;
			}

			path = path != null ? $"{currentPath}_{path}" : currentPath;
			return true;
		}

		/// <summary>
		/// Get the mapped type for the given expression.
		/// </summary>
		/// <param name="parameters">The query parameters.</param>
		/// <param name="expression">The expression.</param>
		/// <returns>The mapped type of the expression or <see langword="null"/> when the mapped type was not
		/// found and the <paramref name="expression"/> type is <see cref="object"/>.</returns>
		internal static IType GetType(VisitorParameters parameters, Expression expression)
		{
			if (expression is ConstantExpression constantExpression &&
				parameters.ConstantToParameterMap.TryGetValue(constantExpression, out var param))
			{
				return param.Type;
			}

			if (TryGetMappedType(parameters.SessionFactory, expression, out var type, out _, out _, out _))
			{
				return type;
			}

			return expression.Type == typeof(object) ? null : TypeFactory.HeuristicType(expression.Type);
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
		/// When the <paramref name="expression"/> is polymorphic, the first implementor will be returned.
		/// When the <paramref name="expression"/> contains a <see cref="ConditionalExpression"/>, the first found entity name
		/// will be returned from <see cref="ConditionalExpression.IfTrue"/> or <see cref="ConditionalExpression.IfFalse"/>.
		/// When the <paramref name="expression"/> contains a <see cref="ExpressionType.Coalesce"/> expression, the first found entity name
		/// will be returned from <see cref="BinaryExpression.Left"/> or <see cref="BinaryExpression.Right"/>.
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
			// IEntityNameProvider instance, from which we can retrieve the starting entity name. Once we have it, we have to traverse all
			// expressions that we had to traverse in order to find the IEntityNameProvider instance, but in reverse order (bottom to top)
			// and keep tracking the entity name until we reach to top.

			memberPath = null;
			mappedType = null;
			entityPersister = null;
			component = null;
			// Try to retrieve the starting entity name with all members that were traversed in that process.
			if (!MemberMetadataExtractor.TryGetAllMemberMetadata(expression, out var metadataResults))
			{
				// Failed to find the starting entity name, due to:
				// - Unsupported expression
				// - The expression didn't contain the IEntityNameProvider instance
				return false;
			}

			// Due to coalesce and conditional expressions we can have multiple paths to traverse, in that case find the first path
			// for which we are able to determine the mapped type.
			foreach (var metadataResult in metadataResults)
			{
				if (ProcessMembersMetadataResult(
					metadataResult,
					sessionFactory,
					out mappedType,
					out entityPersister,
					out component,
					out memberPath))
				{
					return true;
				}
			}

			return false;
		}

		private static bool ProcessMembersMetadataResult(
			MemberMetadataResult metadataResult,
			ISessionFactoryImplementor sessionFactory,
			out IType mappedType,
			out IEntityPersister entityPersister,
			out IAbstractComponentType component,
			out string memberPath)
		{
			if (!TryGetEntityPersister(metadataResult.EntityName, null, sessionFactory, out var currentEntityPersister))
			{
				// Failed to find the starting entity name, due to:
				// - Querying a type that is not related to any entity e.g. s.Query<NotRelatedType>().Where(a => a.Type == "A")
				memberPath = null;
				mappedType = null;
				entityPersister = null;
				component = null;
				return false;
			}

			if (metadataResult.MemberPaths.Count == 0) // The expression do not contain any member expressions
			{
				if (metadataResult.ConvertType != null)
				{
					mappedType = TryGetEntityPersister(
						currentEntityPersister,
						metadataResult.ConvertType,
						sessionFactory,
						out var convertPersister)
						? convertPersister.EntityMetamodel.EntityType // ((Subclass)q)
						: TypeFactory.GetDefaultTypeFor(metadataResult.ConvertType); // ((NotMapped)q)
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
			if (metadataResult.ConvertType != null)
			{
				if (!TryGetEntityPersister(
					currentEntityPersister,
					metadataResult.ConvertType,
					sessionFactory,
					out var convertPersister)) // ((NotMapped)q).Id
				{
					memberPath = null;
					mappedType = null;
					entityPersister = null;
					component = null;
					return false;
				}

				currentEntityPersister = convertPersister; // ((Subclass)q).Id
			}

			return TraverseMembers(
				sessionFactory,
				metadataResult.MemberPaths,
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
				memberPath = member.Path;
				var convertType = member.ConvertType;
				member = memberPaths.Pop();

				switch (currentType)
				{
					case IAssociationType associationType:
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
						currentComponentType = componentType;
						if (currentEntityPersister == null)
						{
							// When persister is not available (q.OneToManyCompositeElement[0].Prop), try to get the type from the component
							currentType = TryGetComponentPropertyType(componentType, member.Path);
						}
						else
						{
							// Concatenate the component property path in order to be able to use EntityMetamodel.GetPropertyType to retrieve the type.
							// As GetPropertyType supports only components, do not concatenate when dealing with collection composite elements or elements.
							// q.Component.Prop
							member = new MemberMetadata(
								$"{memberPath}.{member.Path}",
								member.ConvertType,
								member.HasIndexer);

							// q.Component.Prop
							currentType = currentEntityPersister.EntityMetamodel.GetPropertyType(member.Path);
						}

						break;
					default:
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
				mappedType = GetType(currentEntityPersister, currentType, member, sessionFactory);
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

		private static IType TryGetComponentPropertyType(IAbstractComponentType componentType, string memberPath)
		{
			var index = Array.IndexOf(componentType.PropertyNames, memberPath);
			return index < 0
				? null // q.OneToManyCompositeElement[0].NotMappedProp
				: componentType.Subtypes[index]; // q.OneToManyCompositeElement[0].Prop
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
							memberType = TryGetComponentPropertyType(componentType, member.Path);
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
				// When dealing with a polymorphic query it is not important which entity name we pick
				// as they all need to have the same mapped types for members of the type that is queried.
				// If one of the entites has a different type mapped (e.g. enum mapped as string instead of numeric),
				// the query will fail to execute as currently the ParameterMetadata is bound to IQueryPlan and not to IQueryTranslator
				// (e.g. s.Query<IEntity>().Where(a => a.MyEnum == MyEnum.Option)).
				currentEntityName = sessionFactory.GetImplementors(currentEntityName).FirstOrDefault();
				if (currentEntityName == null)
				{
					persister = null;
					return false;
				}

				currentEntityPersister = sessionFactory.GetEntityPersister(currentEntityName);
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
			ISessionFactoryImplementor sessionFactory)
		{
			// Not mapped
			if (currentType == null)
			{
				return null;
			}

			IEntityPersister persister;
			if (!member.HasIndexer || currentEntityPersister == null)
			{
				if (member.ConvertType == null)
				{
					return currentType; // q.Prop, q.OneToManyCompositeElement[0].Prop
				}

				return TryGetEntityPersister(member.ConvertType, sessionFactory, out persister)
					? persister.EntityMetamodel.EntityType // (Entity)q.Prop, (Entity)q.OneToManyCompositeElement[0].Prop
					: TypeFactory.GetDefaultTypeFor(member.ConvertType); // (long)q.Prop, (long)q.OneToManyCompositeElement[0].Prop
			}

			if (!(currentType is IAssociationType associationType))
			{
				// q.Prop[0]
				return null;
			}

			var queryableCollection = (IQueryableCollection) associationType.GetAssociatedJoinable(sessionFactory);
			if (member.ConvertType == null)
			{
				// q.OneToMany[0]
				return queryableCollection.ElementType;
			}

			return TryGetEntityPersister(member.ConvertType, sessionFactory, out persister)
				? persister.EntityMetamodel.EntityType // (Entity)q.OneToMany[0]
				: TypeFactory.GetDefaultTypeFor(member.ConvertType); // (long)q.OneToMany[0]
		}

		private class MemberMetadataExtractor : NhExpressionVisitor
		{
			private readonly List<MemberMetadataResult> _childrenResults = new List<MemberMetadataResult>();
			private readonly Stack<MemberMetadata> _memberPaths;
			private System.Type _convertType;
			private bool _hasIndexer;
			private string _entityName;

			private MemberMetadataExtractor(Stack<MemberMetadata> memberPaths, System.Type convertType, bool hasIndexer)
			{
				_memberPaths = memberPaths;
				_convertType = convertType;
				_hasIndexer = hasIndexer;
			}

			/// <summary>
			/// Traverses the expression from top to bottom until the first <see cref="ConstantExpression"/> containing an IEntityNameProvider
			/// instance is found.
			/// </summary>
			/// <param name="expression">The expression to traverse.</param>
			/// <param name="results">Output parameter that represents a collection, where each item contains information about all
			/// <see cref="MemberExpression"/> that were traversed until the first <see cref="ConstantExpression"/> containing an
			/// <see cref="IEntityNameProvider"/> instance is found. The number of items depends on how many different paths exist
			/// in the <paramref name="expression"/> that contains a <see cref="IEntityNameProvider"/> instance. When <see cref="IEntityNameProvider"/>
			/// is not found or one of the expressions is not supported the parameter will be set to <see langword="null"/>.</param>
			/// <returns>Whether <paramref name="results"/> was populated.</returns>
			public static bool TryGetAllMemberMetadata(Expression expression, out List<MemberMetadataResult> results)
			{
				if (TryGetAllMemberMetadata(expression, new Stack<MemberMetadata>(), null, false, out var result))
				{
					results = result.GetAllResults().ToList();
					return true;
				}

				results = null;
				return false;
			}

			private static bool TryGetAllMemberMetadata(
				Expression expression,
				Stack<MemberMetadata> memberPaths,
				System.Type convertType,
				bool hasIndexer,
				out MemberMetadataResult results)
			{
				var extractor = new MemberMetadataExtractor(memberPaths, convertType, hasIndexer);
				extractor.Accept(expression);
				results = extractor._entityName != null || extractor._childrenResults.Count > 0
					? new MemberMetadataResult(
						extractor._childrenResults,
						extractor._memberPaths,
						extractor._entityName,
						extractor._convertType)
					: null;

				return results != null;
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

#if NETCOREAPP2_0
			protected override Expression VisitInvocation(InvocationExpression node)
			{
				if (TryGetDynamicMemberBinder(node, out var binder))
				{
					_memberPaths.Push(new MemberMetadata(binder.Name, _convertType, _hasIndexer));
					_convertType = null;
					_hasIndexer = false;
					return base.Visit(node.Arguments[1]);
				}

				return base.VisitInvocation(node);
			}
#endif

			protected override Expression VisitDynamic(DynamicExpression node)
			{
				if (node.Binder is GetMemberBinder binder)
				{
					_memberPaths.Push(new MemberMetadata(binder.Name, _convertType, _hasIndexer));
					_convertType = null;
					_hasIndexer = false;
					return base.Visit(node.Arguments[0]);
				}

				return Visit(node);
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

				if (node.ReferencedQuerySource is NhOuterJoinClause outerJoinClause)
				{
					return base.Visit(outerJoinClause.JoinClause.InnerSequence);
				}

				// Not supported expression
				_entityName = null;
				return node;
			}

			protected override Expression VisitSubQuery(SubQueryExpression expression)
			{
				base.Visit(expression.QueryModel.SelectClause.Selector);
				return expression;
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

				if (node.NodeType == ExpressionType.Coalesce &&
				    (TryGetMembersMetadata(node.Left) | TryGetMembersMetadata(node.Right)))
				{
					return node;
				}

				return Visit(node);
			}

			protected override Expression VisitConditional(ConditionalExpression node)
			{
				if (TryGetMembersMetadata(node.IfTrue) | TryGetMembersMetadata(node.IfFalse))
				{
					return node;
				}

				return Visit(node);
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

				if (VisitorUtil.TryGetPotentialDynamicComponentDictionaryMember(node, out var memberName))
				{
					_memberPaths.Push(new MemberMetadata(memberName, _convertType, _hasIndexer));
					_convertType = null;
					_hasIndexer = false;
					return base.Visit(node.Object);
				}

				return Visit(node);
			}

			public override Expression Visit(Expression node)
			{
				// Not supported expression
				_entityName = null;
				return node;
			}

			private bool TryGetMembersMetadata(Expression expression)
			{
				if (TryGetAllMemberMetadata(expression, Clone(_memberPaths), _convertType, _hasIndexer, out var result))
				{
					_childrenResults.Add(result);
					return true;
				}

				return false;
			}

			private static Stack<T> Clone<T>(Stack<T> original)
			{
				var arr = new T[original.Count];
				original.CopyTo(arr, 0);
				Array.Reverse(arr);
				return new Stack<T>(arr);
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

		private class MemberMetadataResult
		{
			public MemberMetadataResult(
				List<MemberMetadataResult> childrenResults,
				Stack<MemberMetadata> memberPaths,
				string entityName,
				System.Type convertType)
			{
				ChildrenResults = childrenResults;
				MemberPaths = memberPaths;
				EntityName = entityName;
				ConvertType = convertType;
			}

			/// <summary>
			/// Metadata about all <see cref="MemberExpression"/> that were traversed.
			/// </summary>
			public Stack<MemberMetadata> MemberPaths { get; }

			/// <summary>
			/// <see cref="UnaryExpression"/> type that was used on a <see cref="ConstantExpression"/> containing
			/// an <see cref="IEntityNameProvider"/>.
			/// </summary>
			public System.Type ConvertType { get; }

			/// <summary>
			/// The entity name from <see cref="IEntityNameProvider.EntityName"/>.
			/// </summary>
			public string EntityName { get; }

			/// <summary>
			/// Direct children of the current metadata result.
			/// </summary>
			public List<MemberMetadataResult> ChildrenResults { get; }

			/// <summary>
			/// Gets all leaf (bottom) children that have the entity name set.
			/// </summary>
			/// <returns></returns>
			public IEnumerable<MemberMetadataResult> GetAllResults()
			{
				return GetAllResults(this);
			}

			private static IEnumerable<MemberMetadataResult> GetAllResults(MemberMetadataResult result)
			{
				if (result.ChildrenResults.Count == 0)
				{
					yield return result;
				}
				else
				{
					foreach (var childResult in result.ChildrenResults)
					{
						foreach (var childChildrenResult in GetAllResults(childResult))
						{
							yield return childChildrenResult;
						}
					}
				}
			}
		}
	}
}
