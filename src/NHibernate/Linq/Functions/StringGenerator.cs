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
	public class LikeGenerator : IHqlGeneratorForMethod, IRuntimeMethodHqlGenerator, IHqlGeneratorForMethodExtended
	{
		public IEnumerable<MethodInfo> SupportedMethods
		{
			get { throw new NotImplementedException(); }
		}

		public HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments,
		                            HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			if (arguments.Count == 2)
			{
				return treeBuilder.Like(
					visitor.Visit(arguments[0]).AsExpression(),
					visitor.Visit(arguments[1]).AsExpression());
			}
			if (arguments[2].NodeType == ExpressionType.Constant)
			{
				var escapeCharExpression = (ConstantExpression)arguments[2];
				return treeBuilder.Like(
					visitor.Visit(arguments[0]).AsExpression(),
					visitor.Visit(arguments[1]).AsExpression(),
					treeBuilder.Constant(escapeCharExpression.Value));
			}
			throw new ArgumentException("The escape character must be specified as literal value or a string variable");
		}

		public bool SupportsMethod(MethodInfo method)
		{
			// This will match the following methods:
			//   NHibernate.Linq.SqlMethods.Like(string, string)
			//   System.Data.Linq.SqlClient.SqlMethods.Like(string, string) (from Linq2SQL)
			//   plus any 2-argument method named Like in a class named SqlMethods
			//
			// The latter case allows application developers to define their own placeholder method
			// to avoid referencing Linq2Sql or Linq2NHibernate, if they so wish.

			return method != null && method.Name == "Like" &&
			       (method.GetParameters().Length == 2 || method.GetParameters().Length == 3) &&
			       method.DeclaringType != null &&
			       method.DeclaringType.FullName.EndsWith("SqlMethods");
		}

		public IHqlGeneratorForMethod GetMethodGenerator(MethodInfo method)
		{
			return this;
		}

		public bool AllowsNullableReturnType(MethodInfo method) => false;

		/// <inheritdoc />
		public bool TryGetCollectionParameter(MethodCallExpression expression, out ConstantExpression collectionParameter)
		{
			collectionParameter = null;
			return false;
		}
	}

	public class LengthGenerator : BaseHqlGeneratorForProperty
	{
		public LengthGenerator()
		{
			SupportedProperties = new[] { ReflectHelper.GetProperty((string x) => x.Length) };
		}

		public override HqlTreeNode BuildHql(MemberInfo member, Expression expression, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			return treeBuilder.MethodCall("length", visitor.Visit(expression).AsExpression());
		}
	}

	public class StartsWithGenerator : BaseHqlGeneratorForMethod
	{
		private static readonly MethodInfo MethodWithComparer = ReflectHelper.GetMethodDefinition<string>(x => x.StartsWith(null, default(StringComparison)));

		public StartsWithGenerator()
		{
			SupportedMethods = new[] {ReflectHelper.GetMethodDefinition<string>(x => x.StartsWith(null)), MethodWithComparer};
		}

		public override bool AllowsNullableReturnType(MethodInfo method) => false;

		public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			LogIgnoredStringComparisonParameter(method, MethodWithComparer);
			return treeBuilder.Like(
					visitor.Visit(targetObject).AsExpression(),
					treeBuilder.Concat(
							visitor.Visit(arguments[0]).AsExpression(),
							treeBuilder.Constant("%")));
		}
	}

	public class EndsWithGenerator : BaseHqlGeneratorForMethod
	{
		private static readonly MethodInfo MethodWithComparer = ReflectHelper.GetMethodDefinition<string>(x => x.EndsWith(null, default(StringComparison)));

		public EndsWithGenerator()
		{
			SupportedMethods = new[] {ReflectHelper.GetMethodDefinition<string>(x => x.EndsWith(null)), MethodWithComparer,};
		}

		public override bool AllowsNullableReturnType(MethodInfo method) => false;

		public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			LogIgnoredStringComparisonParameter(method, MethodWithComparer);
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
			SupportedMethods = new[] { ReflectHelper.GetMethodDefinition<string>(x => x.Contains(null)) };
		}

		public override bool AllowsNullableReturnType(MethodInfo method) => false;

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

	public class ToLowerGenerator : BaseHqlGeneratorForMethod
	{
		public ToLowerGenerator()
		{
			SupportedMethods = new[]
									{
										ReflectHelper.GetMethodDefinition<string>(x => x.ToLower()),
										ReflectHelper.GetMethodDefinition<string>(x => x.ToLowerInvariant())
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
										ReflectHelper.GetMethodDefinition<string>(x => x.ToUpper()),
										ReflectHelper.GetMethodDefinition<string>(x => x.ToUpperInvariant()),
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
										ReflectHelper.GetMethodDefinition<string>(s => s.Substring(0)),
										ReflectHelper.GetMethodDefinition<string>(s => s.Substring(0, 0))
									};
		}

		public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			var stringExpr = visitor.Visit(targetObject).AsExpression();
			var start = treeBuilder.Add(visitor.Visit(arguments[0]).AsExpression(), treeBuilder.Constant(1));

			if (arguments.Count == 1)
				return treeBuilder.MethodCall("substring", stringExpr, start);

			var length = visitor.Visit(arguments[1]).AsExpression();
			return treeBuilder.MethodCall("substring", stringExpr, start, length);
		}
	}
	
	public class GetCharsGenerator : BaseHqlGeneratorForMethod
	{
		public GetCharsGenerator()
		{
			SupportedMethods = new[]
			{
				ReflectHelper.GetMethod<string, char>(s => s[0])
			};
		}

		public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			var expression = visitor.Visit(targetObject).AsExpression();
			var index = treeBuilder.Add(visitor.Visit(arguments[0]).AsExpression(), treeBuilder.Constant(1));
			return treeBuilder.MethodCall("substring", expression, index, treeBuilder.Constant(1));
		}
	}

	public class IndexOfGenerator : BaseHqlGeneratorForMethod
	{
		private static readonly MethodInfo MethodWithComparer1 = ReflectHelper.GetMethodDefinition<string>(x => x.IndexOf(string.Empty, default(StringComparison)));
		private static readonly MethodInfo MethodWithComparer2 = ReflectHelper.GetMethodDefinition<string>(x => x.IndexOf(string.Empty, 0, default(StringComparison)));

		public IndexOfGenerator()
		{
			SupportedMethods = new[]
									{
										ReflectHelper.GetMethodDefinition<string>(s => s.IndexOf(' ')),
										ReflectHelper.GetMethodDefinition<string>(s => s.IndexOf(" ")),
										ReflectHelper.GetMethodDefinition<string>(s => s.IndexOf(' ', 0)),
										ReflectHelper.GetMethodDefinition<string>(s => s.IndexOf(" ", 0)),
										MethodWithComparer1,
										MethodWithComparer2,
									};
		}
		public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			var argsCount = arguments.Count;
			if (LogIgnoredStringComparisonParameter(method, MethodWithComparer1, MethodWithComparer2))
			{
				//StringComparison is last argument, just ignore it
				argsCount--;
			}

			HqlMethodCall locate;
			if (argsCount == 1)
			{
				locate = treeBuilder.MethodCall("locate",
					visitor.Visit(arguments[0]).AsExpression(),
					visitor.Visit(targetObject).AsExpression()); //,
				//treeBuilder.Constant(0));
			}
			else
			{
				var start = treeBuilder.Add(visitor.Visit(arguments[1]).AsExpression(), treeBuilder.Constant(1));
				locate = treeBuilder.MethodCall("locate",
							visitor.Visit(arguments[0]).AsExpression(),
							visitor.Visit(targetObject).AsExpression(),
							start);
			}
			return treeBuilder.Subtract(locate,treeBuilder.Constant(1));
		}
	}

	public class ReplaceGenerator : BaseHqlGeneratorForMethod
	{
#if NETCOREAPP2_0  
		private static readonly MethodInfo MethodWithComparer = ReflectHelper.GetMethodDefinition<string>(x => x.Replace(string.Empty, string.Empty, default(StringComparison)));
#endif

		public ReplaceGenerator()
		{
			SupportedMethods = new[]
			{
				ReflectHelper.GetMethodDefinition<string>(s => s.Replace(' ', ' ')),
				ReflectHelper.GetMethodDefinition<string>(s => s.Replace("", "")),
#if NETCOREAPP2_0
				MethodWithComparer,
#endif
			};
		}

		public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
#if NETCOREAPP2_0
			LogIgnoredStringComparisonParameter(method, MethodWithComparer);
#endif
			return treeBuilder.MethodCall(
				"replace",
				visitor.Visit(targetObject).AsExpression(),
				visitor.Visit(arguments[0]).AsExpression(),
				visitor.Visit(arguments[1]).AsExpression());
		}
	}

	public class TrimGenerator : BaseHqlGeneratorForMethod
	{
		public TrimGenerator()
		{
			SupportedMethods = typeof(string).GetMethods().Where(x => new[] { "Trim", "TrimStart", "TrimEnd" }.Contains(x.Name)).ToArray();
		}

		public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			string trimWhere;
			if (method.Name == "TrimStart")
				trimWhere = "leading";
			else if (method.Name == "TrimEnd")
				trimWhere = "trailing";
			else
				trimWhere = "both";

			var trimChars = ExtractTrimChars(arguments);

			if (string.IsNullOrEmpty(trimChars))
			{
				return treeBuilder.MethodCall("trim", treeBuilder.Ident(trimWhere), treeBuilder.Ident("from"), visitor.Visit(targetObject).AsExpression());
			}
			else
			{
				return treeBuilder.MethodCall("trim", treeBuilder.Ident(trimWhere), treeBuilder.Constant(trimChars), treeBuilder.Ident("from"), visitor.Visit(targetObject).AsExpression());
			}
		}

		private static string ExtractTrimChars(IReadOnlyList<Expression> arguments)
		{
			if (arguments.Count > 0)
			{
				var constantExpression = (ConstantExpression) arguments[0];
				if (constantExpression.Value is char c)
					return c.ToString();
				if (constantExpression.Value is char[] chars)
					return new string(chars);
			}

			return string.Empty;
		}
	}

	public class ToStringRuntimeMethodHqlGenerator : IRuntimeMethodHqlGenerator
	{
		private readonly ToStringHqlGeneratorForMethod generator = new ToStringHqlGeneratorForMethod();

		public bool SupportsMethod(MethodInfo method)
		{
			return method != null && method.Name == "ToString" && method.GetBaseDefinition().DeclaringType == typeof(object);
		}

		public IHqlGeneratorForMethod GetMethodGenerator(MethodInfo method)
		{
			return generator;
		}
	}

	public class ToStringHqlGeneratorForMethod : IHqlGeneratorForMethod
	{
		public IEnumerable<MethodInfo> SupportedMethods => throw new NotSupportedException();

		public HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			var methodName = targetObject.Type == typeof(Guid) || targetObject.Type == typeof(Guid?)
				? "strguid"
				: "str";

			return treeBuilder.MethodCall(methodName, visitor.Visit(targetObject).AsExpression());
		}
	}
}
