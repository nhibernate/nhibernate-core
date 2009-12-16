using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Visitors;

namespace NHibernate.Linq.Functions
{
    public class QueryableGenerator : BaseHqlGeneratorForType
    {
        public QueryableGenerator()
        {
            // TODO - could use reflection
            MethodRegistry.Add(new AnyGenerator());
            MethodRegistry.Add(new AllGenerator());
            MethodRegistry.Add(new MinGenerator());
            MethodRegistry.Add(new MaxGenerator());
        }

        class AnyGenerator : BaseHqlGeneratorForMethod
        {
            public AnyGenerator()
            {
                SupportedMethods = new[]
                                       {
                                           ReflectionHelper.GetMethod(() => Queryable.Any<object>(null)),
                                           ReflectionHelper.GetMethod(() => Queryable.Any<object>(null, null)),
                                           ReflectionHelper.GetMethod(() => Enumerable.Any<object>(null)),
                                           ReflectionHelper.GetMethod(() => Enumerable.Any<object>(null, null))
                                       };
            }

            public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
            {
                HqlAlias alias = null;
                HqlWhere where = null;

                if (arguments.Count > 1)
                {
                    var expr = (LambdaExpression)arguments[1];

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

        class AllGenerator : BaseHqlGeneratorForMethod
        {
            public AllGenerator()
            {
                SupportedMethods = new[]
                                       {
                                           ReflectionHelper.GetMethod(() => Queryable.All<object>(null, null)),
                                           ReflectionHelper.GetMethod(() => Enumerable.All<object>(null, null))
                                       };
            }

            public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
            {
                // All has two arguments.  Arg 1 is the source and arg 2 is the predicate
                var predicate = (LambdaExpression)arguments[1];

                return treeBuilder.Not(
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
                                treeBuilder.Not(visitor.Visit(arguments[1]).AsBooleanExpression())
                                )
                            )
                        )
                    );
            }
        }

        class MinGenerator : BaseHqlGeneratorForMethod
        {
            public MinGenerator()
            {
                SupportedMethods = new[]
                                       {
                                           ReflectionHelper.GetMethod(() => Queryable.Min<object>(null)),
                                           ReflectionHelper.GetMethod(() => Enumerable.Min<object>(null))
                                       };
            }

            public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
            {
                return treeBuilder.Min(visitor.Visit(arguments[1]).AsExpression());
            }
        }

        class MaxGenerator : BaseHqlGeneratorForMethod
        {
            public MaxGenerator()
            {
                SupportedMethods = new[]
                                       {
                                           ReflectionHelper.GetMethod(() => Queryable.Max<object>(null)),
                                           ReflectionHelper.GetMethod(() => Enumerable.Max<object>(null))
                                       };
            }

            public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
            {
                return treeBuilder.Max(visitor.Visit(arguments[1]).AsExpression());
            }
        }
    }

    public class ICollectionGenerator : BaseHqlGeneratorForType
    {
        public ICollectionGenerator()
        {
            // TODO - could use reflection
            MethodRegistry.Add(new ContainsGenerator());
        }

        public override bool SupportsMethod(MethodInfo method)
        {
            var declaringType = method.DeclaringType;

            if (declaringType.IsGenericType)
            {
                if (declaringType.GetGenericTypeDefinition() == typeof(ICollection<>) ||
                    declaringType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    if (method.Name == "Contains")
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public override IHqlGeneratorForMethod GetMethodGenerator(MethodInfo method)
        {
            // TODO - ick
            if (method.Name == "Contains")
            {
                return new ContainsGenerator();
            }

            throw new NotSupportedException(method.Name);
        }

        class ContainsGenerator : BaseHqlGeneratorForMethod
        {
            public ContainsGenerator()
            {
                SupportedMethods = new MethodInfo[0];
            }

            public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
            {
                // TODO - alias generator
                var alias = treeBuilder.Alias("x");

                var param = Expression.Parameter(targetObject.Type, "x");
                var where = treeBuilder.Where(visitor.Visit(Expression.Lambda(
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
}