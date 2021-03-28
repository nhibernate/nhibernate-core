using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Visitors;
using NHibernate.Util;

namespace NHibernate.Linq.Functions
{
	public class AnyHqlGenerator : BaseHqlGeneratorForMethod
	{
		public AnyHqlGenerator()
		{
			SupportedMethods = new[]
			{
				ReflectionCache.QueryableMethods.AnyDefinition,
				ReflectionCache.QueryableMethods.AnyWithPredicateDefinition,
				ReflectHelper.FastGetMethodDefinition(Enumerable.Any, default(IEnumerable<object>)),
				ReflectHelper.FastGetMethodDefinition(Enumerable.Any, default(IEnumerable<object>), default(Func<object, bool>))
			};
		}

		public override bool AllowsNullableReturnType(MethodInfo method) => false;

		public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			HqlAlias alias = null;
			HqlWhere where = null;

			if (arguments.Count > 1)
			{
				var expr = (LambdaExpression) arguments[1];

				alias = treeBuilder.Alias(expr.Parameters[0].Name);
				where = treeBuilder.Where(visitor.Visit(arguments[1]).AsExpression());
			}

			return treeBuilder.Exists(
				treeBuilder.Query(
					treeBuilder.SelectFrom(
						treeBuilder.From(
							treeBuilder.Range(
								visitor.Visit(arguments[0]),
								alias)
							)
						),
					where));
		}
	}

	public class AllHqlGenerator : BaseHqlGeneratorForMethod
	{
		public AllHqlGenerator()
		{
			SupportedMethods = new[]
			{
				ReflectionCache.QueryableMethods.AllDefinition,
				ReflectHelper.FastGetMethodDefinition(Enumerable.All, default(IEnumerable<object>), default(Func<object, bool>))
			};
		}

		public override bool AllowsNullableReturnType(MethodInfo method) => false;

		public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			// All has two arguments.  Arg 1 is the source and arg 2 is the predicate
			var predicate = (LambdaExpression) arguments[1];

			return treeBuilder.BooleanNot(
				treeBuilder.Exists(
					treeBuilder.Query(
						treeBuilder.SelectFrom(
							treeBuilder.From(
								treeBuilder.Range(
									visitor.Visit(arguments[0]),
									treeBuilder.Alias(predicate.Parameters[0].Name))
								)
							),
						treeBuilder.Where(
							treeBuilder.BooleanNot(visitor.Visit(arguments[1]).ToBooleanExpression())
							)
						)
					)
				);
		}
	}

	public class MinHqlGenerator : BaseHqlGeneratorForMethod
	{
		public MinHqlGenerator()
		{
			SupportedMethods = new[]
			{
				ReflectionCache.QueryableMethods.MinDefinition,
				ReflectionCache.EnumerableMethods.MinDefinition
			};
		}

		public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			return treeBuilder.Min(visitor.Visit(arguments[1]).AsExpression());
		}
	}

	public class MaxHqlGenerator : BaseHqlGeneratorForMethod
	{
		public MaxHqlGenerator()
		{
			SupportedMethods = new[]
			{
				ReflectionCache.QueryableMethods.MaxDefinition,
				ReflectionCache.EnumerableMethods.MaxDefinition,
			};
		}

		public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			return treeBuilder.Max(visitor.Visit(arguments[1]).AsExpression());
		}
	}

	public class CollectionContainsRuntimeHqlGenerator : IRuntimeMethodHqlGenerator
	{
		private readonly IHqlGeneratorForMethod containsGenerator = new CollectionContainsGenerator();

		#region IRuntimeMethodHqlGenerator Members

		public bool SupportsMethod(MethodInfo method)
		{
			// the check about the name is to make things a little be fasters
			return method != null && method.Name == "Contains" && method.IsMethodOf(typeof(ICollection<>));
		}

		public IHqlGeneratorForMethod GetMethodGenerator(MethodInfo method)
		{
			return containsGenerator;
		}

		#endregion
	}

	public class CollectionContainsGenerator : BaseHqlGeneratorForMethod
	{
		public CollectionContainsGenerator()
		{
			SupportedMethods = new[]
			{
				ReflectHelper.FastGetMethodDefinition(Queryable.Contains, default(IQueryable<object>), default(object)),
				ReflectHelper.FastGetMethodDefinition(Enumerable.Contains, default(IEnumerable<object>), default(object))
			};
		}

		public override bool AllowsNullableReturnType(MethodInfo method) => false;

		/// <inheritdoc />
		public override bool TryGetCollectionParameter(MethodCallExpression expression, out ConstantExpression collectionParameter)
		{
			var argument = expression.Method.IsStatic ? expression.Arguments[0] : expression.Object;
			collectionParameter = argument as ConstantExpression;

			return collectionParameter != null;
		}

		public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			// TODO - alias generator
			HqlAlias alias = treeBuilder.Alias("x");

			ParameterExpression param = Expression.Parameter(targetObject.Type, "x");
			HqlWhere where = treeBuilder.Where(visitor.Visit(Expression.Lambda(
				Expression.Equal(param, arguments[0]), param))
			                                   	.AsExpression());

			return treeBuilder.Exists(
				treeBuilder.Query(
					treeBuilder.SelectFrom(
						treeBuilder.From(
							treeBuilder.Range(
								visitor.Visit(targetObject),
								alias)
							)
						),
					where));
		}
	}
}
