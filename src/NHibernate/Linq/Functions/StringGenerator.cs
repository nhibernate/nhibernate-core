using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Visitors;

namespace NHibernate.Linq.Functions
{
		public class LengthGenerator : BaseHqlGeneratorForProperty
		{
			public LengthGenerator()
			{
				SupportedProperties = new[] { ReflectionHelper.GetProperty((string x) => x.Length) };
			}

			public override HqlTreeNode BuildHql(MemberInfo member, Expression expression, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
			{
				return treeBuilder.MethodCall("length", visitor.Visit(expression).AsExpression());
			}
		}

		public class StartsWithGenerator : BaseHqlGeneratorForMethod
		{
			public StartsWithGenerator()
			{
				SupportedMethods = new[] { ReflectionHelper.GetMethodDefinition<string>(x => x.StartsWith(null)) };
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

		public class EndsWithGenerator : BaseHqlGeneratorForMethod
		{
			public EndsWithGenerator()
			{
				SupportedMethods = new[] { ReflectionHelper.GetMethodDefinition<string>(x => x.EndsWith(null)) };
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

		public class ContainsGenerator : BaseHqlGeneratorForMethod
		{
			public ContainsGenerator()
			{
				SupportedMethods = new[] { ReflectionHelper.GetMethodDefinition<string>(x => x.Contains(null)) };
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

		public class EqualsGenerator : BaseHqlGeneratorForMethod
		{
			public EqualsGenerator()
			{
				SupportedMethods = new[] { ReflectionHelper.GetMethodDefinition<string>(x => x.Equals((string)null)) };
			}

			public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
			{
				return treeBuilder.Equality(
						visitor.Visit(targetObject).AsExpression(),
						visitor.Visit(arguments[0]).AsExpression());
			}
		}

		public class ToLowerGenerator : BaseHqlGeneratorForMethod
		{
			public ToLowerGenerator()
			{
				SupportedMethods = new[]
                                       {
                                           ReflectionHelper.GetMethodDefinition<string>(x => x.ToLower()),
                                           ReflectionHelper.GetMethodDefinition<string>(x => x.ToLowerInvariant())
                                       };
			}

			public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
			{
				return treeBuilder.MethodCall("lower", visitor.Visit(targetObject).AsExpression());
			}
		}

		public class ToUpperGenerator : BaseHqlGeneratorForMethod
		{
			public ToUpperGenerator()
			{
				SupportedMethods = new[]
                                       {
                                           ReflectionHelper.GetMethodDefinition<string>(x => x.ToUpper()),
                                           ReflectionHelper.GetMethodDefinition<string>(x => x.ToUpperInvariant()),
                                       };
			}

			public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
			{
				return treeBuilder.MethodCall("upper", visitor.Visit(targetObject).AsExpression());
			}
		}

		public class SubStringGenerator : BaseHqlGeneratorForMethod
		{
			public SubStringGenerator()
			{
				SupportedMethods = new[]
                                       {
                                           ReflectionHelper.GetMethodDefinition<string>(s => s.Substring(0)),
                                           ReflectionHelper.GetMethodDefinition<string>(s => s.Substring(0, 0))
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

		public class IndexOfGenerator : BaseHqlGeneratorForMethod
		{
			public IndexOfGenerator()
			{
				SupportedMethods = new[]
                                       {
                                           ReflectionHelper.GetMethodDefinition<string>(s => s.IndexOf(' ')),
                                           ReflectionHelper.GetMethodDefinition<string>(s => s.IndexOf(" ")),
                                           ReflectionHelper.GetMethodDefinition<string>(s => s.IndexOf(' ', 0)),
                                           ReflectionHelper.GetMethodDefinition<string>(s => s.IndexOf(" ", 0))
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

		public class ReplaceGenerator : BaseHqlGeneratorForMethod
		{
			public ReplaceGenerator()
			{
				SupportedMethods = new[]
                                       {
                                           ReflectionHelper.GetMethodDefinition<string>(s => s.Replace(' ', ' ')),
                                           ReflectionHelper.GetMethodDefinition<string>(s => s.Replace("", ""))
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