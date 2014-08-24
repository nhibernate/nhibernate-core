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
			                   		ReflectionHelper.GetMethodDefinition(() => Queryable.Any<object>(null)),
			                   		ReflectionHelper.GetMethodDefinition(() => Queryable.Any<object>(null, null)),
			                   		ReflectionHelper.GetMethodDefinition(() => Enumerable.Any<object>(null)),
			                   		ReflectionHelper.GetMethodDefinition(() => Enumerable.Any<object>(null, null))
			                   	};
		}

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
			                   		ReflectionHelper.GetMethodDefinition(() => Queryable.All<object>(null, null)),
			                   		ReflectionHelper.GetMethodDefinition(() => Enumerable.All<object>(null, null))
			                   	};
		}

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
							treeBuilder.BooleanNot(visitor.Visit(arguments[1]).AsBooleanExpression())
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
			                   		ReflectionHelper.GetMethodDefinition(() => Queryable.Min<object>(null)),
			                   		ReflectionHelper.GetMethodDefinition(() => Enumerable.Min<object>(null))
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
			                   		ReflectionHelper.GetMethodDefinition(() => Queryable.Max<object>(null)),
			                   		ReflectionHelper.GetMethodDefinition(() => Enumerable.Max<object>(null))
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
			                   		ReflectionHelper.GetMethodDefinition(() => Queryable.Contains<object>(null, null)),
			                   		ReflectionHelper.GetMethodDefinition(() => Enumerable.Contains<object>(null, null))
			                   	};
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