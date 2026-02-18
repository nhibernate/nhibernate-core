using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Linq.Clauses;
using NHibernate.Linq.GroupBy;
using NHibernate.Linq.Visitors;
using NHibernate.Util;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;

namespace NHibernate.Linq.NestedSelects
{
	/// <summary>
	/// Recursive visitor that rewrites nested collection expands (e.g. OData $expand=Orders/OrderLines)
	/// into a chain of GroupBy/Select expressions that NHibernate can translate to SQL.
	/// Ported from the COS Systems NHibernateAll 4.0.4.4002 custom build.
	/// </summary>
	internal class NestedSelectClauseRewriter : RelinqExpressionVisitor
	{
		private static readonly MethodInfo CastMethod =
			ReflectHelper.FastGetMethod(Enumerable.Cast<object[]>, default(IEnumerable));

		private static readonly MethodInfo GroupByMethod =
			ReflectHelper.FastGetMethod(Enumerable.GroupBy<object[], object>,
				default(IEnumerable<object[]>), default(Func<object[], object>));

		private static readonly MethodInfo SingleMethod =
			ReflectHelper.FastGetMethod(Enumerable.Single<IGrouping<object, object[]>>,
				default(IEnumerable<IGrouping<object, object[]>>));

		private static readonly MethodInfo WhereMethod =
			ReflectHelper.FastGetMethod(Enumerable.Where<IGrouping<object, object[]>>,
				default(IEnumerable<IGrouping<object, object[]>>), default(Func<IGrouping<object, object[]>, bool>));

		private readonly ISessionFactory _sessionFactory;
		private readonly List<Expression> _expressions;

		public NestedSelectClauseRewriter Parent { get; set; }
		public ParameterExpression Source { get; set; }
		public QueryModel QueryModel { get; set; }

		public List<Expression> Expressions
		{
			get
			{
				if (Parent == null)
					return _expressions;
				return Parent.Expressions;
			}
		}

		public int Level
		{
			get
			{
				if (Parent != null)
					return 1 + Parent.Level;
				return 0;
			}
		}

		private static MethodInfo SelectMethod(System.Type type)
		{
			return ReflectionCache.EnumerableMethods.SelectDefinition
				.MakeGenericMethod(typeof(IGrouping<object, object[]>), type);
		}

		private static MethodInfo ToListMethod(System.Type type)
		{
			return ReflectionCache.EnumerableMethods.ToListDefinition.MakeGenericMethod(type);
		}

		private static MethodInfo ToArrayMethod(System.Type type)
		{
			return ReflectionCache.EnumerableMethods.ToArrayDefinition.MakeGenericMethod(type);
		}

		private static LambdaExpression NullFilterPredicate()
		{
			var t = Expression.Parameter(typeof(IGrouping<object, object[]>), "t");
			return Expression.Lambda(
				Expression.Not(
					Expression.Call(typeof(object), "ReferenceEquals", System.Type.EmptyTypes,
						GetKeyProperty(t),
						Expression.Constant(null))),
				t);
		}

		public NestedSelectClauseRewriter(ISessionFactory sessionFactory, QueryModel queryModel,
			NestedSelectClauseRewriter parent = null)
		{
			_sessionFactory = sessionFactory;
			QueryModel = queryModel;
			Parent = parent;

			if (parent == null)
			{
				_expressions = new List<Expression>();
				Expressions.Add(new QuerySourceReferenceExpression(queryModel.MainFromClause));
			}
			else
			{
				var mainFromClause = queryModel.MainFromClause;
				var restrictions = queryModel.BodyClauses.OfType<WhereClause>()
					.Select(w => new NhWithClause(w.Predicate));
				var join = new NhJoinClause(mainFromClause.ItemName, mainFromClause.ItemType,
					mainFromClause.FromExpression, restrictions);
				var root = GetRoot();
				root.BodyClauses.Add(join);
				var swapVisitor = new SwapQuerySourceVisitor(mainFromClause, join);
				root.TransformExpressions(swapVisitor.Swap);
				Expressions.Add(new QuerySourceReferenceExpression(join));
			}

			Source = Expression.Parameter(typeof(IGrouping<object, object[]>), $"level{Level}");
		}

		protected QueryModel GetRoot()
		{
			if (Parent == null)
				return QueryModel;
			return Parent.GetRoot();
		}

		public Expression Start()
		{
			int index = Expressions.Count - 1;

			if (Parent == null)
			{
				var expression = Visit(QueryModel.SelectClause.Selector);
				var input = Expression.Parameter(typeof(IEnumerable<object>), "input");
				var selector = Expression.Lambda(expression, Source);
				var body = Select(selector, GetGroupBy(input, index), expression.Type);
				return Expression.Lambda(body, input);
			}

			var expression2 = Visit(QueryModel.SelectClause.Selector);
			return Select(Expression.Lambda(expression2, Source), GetGroupBy(Parent.Source, index), expression2.Type);
		}

		private MethodCallExpression Select(Expression selector, Expression input, System.Type elementType)
		{
			return CallAsReadOnly(elementType, CallNullFilteredSelect(selector, input, elementType));
		}

		private static MethodCallExpression CallNullFilteredSelect(Expression selector, Expression input, System.Type elementType)
		{
			return Expression.Call(SelectMethod(elementType),
				Expression.Call(WhereMethod, input, NullFilterPredicate()),
				selector);
		}

		private Expression SelectCollection(Expression selector, Expression input, System.Type collectionType, System.Type elementType)
		{
			var filtered = CallNullFilteredSelect(selector, input, elementType);
			if (collectionType.IsArray)
				return Expression.Call(ToArrayMethod(elementType), filtered);
			var constructor = GetCollectionConstructor(collectionType, elementType);
			if (constructor != null)
				return Expression.New(constructor, filtered);
			return CallAsReadOnly(elementType, filtered);
		}

		private static MethodCallExpression CallAsReadOnly(System.Type elementType, MethodCallExpression select)
		{
			return Expression.Call(
				Expression.Call(ToListMethod(elementType), select),
				"AsReadOnly",
				System.Type.EmptyTypes);
		}

		private static MethodCallExpression GetGroupBy(Expression input, int index)
		{
			return Expression.Call(GroupByMethod,
				Expression.Call(CastMethod, input),
				GetArrayIndex(index));
		}

		private static LambdaExpression GetArrayIndex(int index)
		{
			var g = Expression.Parameter(typeof(object[]), "g");
			return Expression.Lambda(Expression.ArrayIndex(g, Expression.Constant(index)), g);
		}

		protected override Expression VisitSubQuery(SubQueryExpression expression)
		{
			var child = new NestedSelectClauseRewriter(_sessionFactory, expression.QueryModel, this);
			var result = child.Start();
			foreach (var expr in child.Expressions)
				AddIfMissing(expr);
			return result;
		}

		private static ConstructorInfo GetCollectionConstructor(System.Type collectionType, System.Type elementType)
		{
			if (collectionType.IsInterface)
			{
				if (collectionType.IsGenericType && collectionType.GetGenericTypeDefinition() == typeof(ISet<>))
					return typeof(HashSet<>).MakeGenericType(elementType)
						.GetConstructor(new[] {typeof(IEnumerable<>).MakeGenericType(elementType)});
				return null;
			}

			return collectionType.GetConstructor(new[] {typeof(IEnumerable<>).MakeGenericType(elementType)});
		}

		protected override Expression VisitMember(MemberExpression expression)
		{
			if (expression.Expression is QuerySourceReferenceExpression)
			{
				var memberType = ReflectHelper.GetPropertyOrFieldType(expression.Member);
				if (memberType != null && memberType.IsCollectionType()
				    && IsMappedCollection(expression.Member) && memberType.IsGenericType)
				{
					var elementType = memberType.GetGenericArguments()[0];
					int index = JoinAndSaveExpression(expression, GetElementType(expression.Type));
					var s = Expression.Parameter(typeof(IGrouping<object, object[]>), "s");
					var selector = Expression.Lambda(Expression.Convert(GetKeyProperty(s), elementType), s);
					return SelectCollection(selector, GetGroupBy(Source, index), memberType, elementType);
				}

				if (memberType != null && IsMapped(expression.Type))
				{
					int index = JoinAndSaveExpression(expression, expression.Type);
					return Expression.Convert(
						GetKeyProperty(Expression.Call(SingleMethod, GetGroupBy(Source, index))),
						memberType);
				}
			}
			else if (expression.Expression is MemberExpression parentMember)
			{
				var memberType = ReflectHelper.GetPropertyOrFieldType(expression.Member);
				if (memberType != null && memberType.IsCollectionType() && IsMappedCollection(expression.Member))
				{
					int index = JoinAndSaveExpression(expression, GetElementType(expression.Type));
					var groupByParent = GetGroupByParent(parentMember, Source,
						FindParentIndex(index - 1, parentMember.Type));
					var elementType = memberType.GetGenericArguments()[0];
					var s = Expression.Parameter(typeof(IGrouping<object, object[]>), "s");
					var selector = Expression.Lambda(Expression.Convert(GetKeyProperty(s), elementType), s);
					return SelectCollection(selector, GetGroupBy(groupByParent, index), memberType, elementType);
				}

				if (memberType != null && IsMapped(expression.Type))
				{
					int index = JoinAndSaveExpression(expression, expression.Type);
					var groupByParent = GetGroupByParent(parentMember, Source,
						FindParentIndex(index - 1, parentMember.Type));
					return Expression.Convert(
						GetKeyProperty(Expression.Call(SingleMethod, GetGroupBy(groupByParent, index))),
						memberType);
				}
			}

			return base.VisitMember(expression);
		}

		private int FindParentIndex(int startIndex, System.Type parentType)
		{
			if (Expressions[startIndex].Type == parentType)
				return startIndex;
			return FindParentIndex(startIndex - 1, parentType);
		}

		private Expression GetGroupByParent(MemberExpression parent, Expression source, int index)
		{
			if (parent.Expression is MemberExpression grandParent)
				source = GetGroupByParent(grandParent, source, FindParentIndex(index - 1, grandParent.Type));
			return Expression.Call(SingleMethod, GetGroupBy(source, index));
		}

		private int JoinAndSaveExpression(MemberExpression expression, System.Type type)
		{
			var join = new NhJoinClause(new NameGenerator(QueryModel).GetNewName(), type, expression);
			GetRoot().BodyClauses.Add(join);
			return AddIfMissing(new QuerySourceReferenceExpression(join));
		}

		private int AddIfMissing(Expression expression)
		{
			if (!Expressions.Contains(expression))
				Expressions.Add(expression);
			return Expressions.IndexOf(expression);
		}

		protected override Expression VisitQuerySourceReference(QuerySourceReferenceExpression expression)
		{
			AddIfMissing(expression);
			return Expression.Convert(GetKeyProperty(Source), expression.Type);
		}

		private static Expression GetKeyProperty(Expression source)
		{
			return Expression.Property(source, "Key");
		}

		private static System.Type GetElementType(System.Type type)
		{
			var elementType = ReflectHelper.GetCollectionElementType(type);
			if (elementType == null)
				throw new NotSupportedException("Unknown collection type " + type.FullName);
			return elementType;
		}

		private bool IsMappedCollection(MemberInfo memberInfo)
		{
			var roleName = memberInfo.DeclaringType.FullName + "." + memberInfo.Name;
			return _sessionFactory.GetCollectionMetadata(roleName) != null;
		}

		private bool IsMapped(System.Type type)
		{
			return _sessionFactory.GetClassMetadata(type.FullName) != null;
		}
	}
}
