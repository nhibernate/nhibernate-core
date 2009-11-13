using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Visitors;

namespace NHibernate.Linq.Functions
{
    public class StringGenerator : BaseHqlGeneratorForType
    {
        public StringGenerator()
        {
            // TODO - could use reflection
            MethodRegistry.Add(new StartsWithGenerator());
            MethodRegistry.Add(new EndsWithGenerator());
            MethodRegistry.Add(new ContainsGenerator());
            MethodRegistry.Add(new EqualsGenerator());
            MethodRegistry.Add(new ToUpperLowerGenerator());
            MethodRegistry.Add(new SubStringGenerator());
            MethodRegistry.Add(new IndexOfGenerator());
            MethodRegistry.Add(new ReplaceGenerator());

            PropertyRegistry.Add(new LengthGenerator());
        }

        public class LengthGenerator : BaseHqlGeneratorForProperty
        {
            public LengthGenerator()
            {
                SupportedProperties = new[] {ReflectionHelper.GetProperty((string x) => x.Length)};
            }

            public override HqlTreeNode BuildHql(MemberInfo member, Expression expression, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
            {
                return treeBuilder.MethodCall("length", visitor.Visit(expression).AsExpression());
            }
        }

        class StartsWithGenerator : BaseHqlGeneratorForMethod
        {
            public StartsWithGenerator()
            {
                SupportedMethods = new[] { ReflectionHelper.GetMethod<string>(x => x.StartsWith(null)) };
            }

            public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
            {
                return treeBuilder.Like(
                    visitor.Visit(targetObject).AsExpression(),
                    treeBuilder.Concat(
                        visitor.Visit(arguments[0]).AsExpression(),
                        treeBuilder.Constant("%")));
            }
        }

        class EndsWithGenerator : BaseHqlGeneratorForMethod
        {
            public EndsWithGenerator()
            {
                SupportedMethods = new[] { ReflectionHelper.GetMethod<string>(x => x.EndsWith(null)) };
            }

            public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
            {
                return treeBuilder.Like(
                    visitor.Visit(targetObject).AsExpression(),
                    treeBuilder.Concat(
                        treeBuilder.Constant("%"),
                        visitor.Visit(arguments[0]).AsExpression()));
            }
        }

        class ContainsGenerator : BaseHqlGeneratorForMethod
        {
            public ContainsGenerator()
            {
                SupportedMethods = new[] { ReflectionHelper.GetMethod<string>(x => x.Contains(null)) };
            }

            public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
            {
                return treeBuilder.Like(
                    visitor.Visit(targetObject).AsExpression(),
                    treeBuilder.Concat(
                        treeBuilder.Constant("%"),
                        visitor.Visit(arguments[0]).AsExpression(),
                        treeBuilder.Constant("%")));
            }
        }

        class EqualsGenerator : BaseHqlGeneratorForMethod
        {
            public EqualsGenerator()
            {
                SupportedMethods = new[] { ReflectionHelper.GetMethod<string>(x => x.Equals((string)null)) };
            }

            public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
            {
                return treeBuilder.Equality(
                    visitor.Visit(targetObject).AsExpression(),
                    visitor.Visit(arguments[0]).AsExpression());
            }
        }

        class ToUpperLowerGenerator : BaseHqlGeneratorForMethod
        {
            public ToUpperLowerGenerator()
            {
                SupportedMethods = new[]
                                       {
                                           ReflectionHelper.GetMethod<string>(x => x.ToUpper()),
                                           ReflectionHelper.GetMethod<string>(x => x.ToUpperInvariant()),
                                           ReflectionHelper.GetMethod<string>(x => x.ToLower()),
                                           ReflectionHelper.GetMethod<string>(x => x.ToLowerInvariant())
                                       };
            }

            public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
            {
                string methodName;

                if (((method.Name == "ToUpper") || (method.Name == "ToUpperInvariant")))
                {
                    methodName = "lower";
                }
                else
                {
                    methodName = "upper";
                }

                return treeBuilder.MethodCall(methodName, visitor.Visit(targetObject).AsExpression());
            }
        }

        class SubStringGenerator : BaseHqlGeneratorForMethod
        {
            public SubStringGenerator()
            {
                SupportedMethods = new[]
                                       {
                                           ReflectionHelper.GetMethod<string>(s => s.Substring(0)),
                                           ReflectionHelper.GetMethod<string>(s => s.Substring(0, 0))
                                       };
            }
            public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
            {
                if (arguments.Count == 1)
                {
                    return treeBuilder.MethodCall("substring", visitor.Visit(targetObject).AsExpression(), 
                        treeBuilder.Constant(0), 
                        visitor.Visit(arguments[0]).AsExpression());
                }
                
                return treeBuilder.MethodCall("substring", visitor.Visit(targetObject).AsExpression(), 
                    visitor.Visit(arguments[0]).AsExpression(), 
                    visitor.Visit(arguments[1]).AsExpression());
            }
        }

        class IndexOfGenerator : BaseHqlGeneratorForMethod
        {
            public IndexOfGenerator()
            {
                SupportedMethods = new[]
                                       {
                                           ReflectionHelper.GetMethod<string>(s => s.IndexOf(' ')),
                                           ReflectionHelper.GetMethod<string>(s => s.IndexOf(" ")),
                                           ReflectionHelper.GetMethod<string>(s => s.IndexOf(' ', 0)),
                                           ReflectionHelper.GetMethod<string>(s => s.IndexOf(" ", 0))
                                       };
            }
            public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
            {
                if (arguments.Count == 1)
                {
                    return treeBuilder.MethodCall("locate", 
                        visitor.Visit(arguments[0]).AsExpression(),
                        visitor.Visit(targetObject).AsExpression(),
                        treeBuilder.Constant(0));
                }
                return treeBuilder.MethodCall("locate", 
                    visitor.Visit(arguments[0]).AsExpression(),
                    visitor.Visit(targetObject).AsExpression(),
                    visitor.Visit(arguments[1]).AsExpression());
            }
        }

        class ReplaceGenerator : BaseHqlGeneratorForMethod
        {
            public ReplaceGenerator()
            {
                SupportedMethods = new[]
                                       {
                                           ReflectionHelper.GetMethod<string>(s => s.Replace(' ', ' ')),
                                           ReflectionHelper.GetMethod<string>(s => s.Replace("", ""))
                                       };
            }

            public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
            {
                return treeBuilder.MethodCall("replace",
                                              visitor.Visit(targetObject).AsExpression(),
                                              visitor.Visit(arguments[0]).AsExpression(),
                                              visitor.Visit(arguments[1]).AsExpression());
            }
        }
    }
}