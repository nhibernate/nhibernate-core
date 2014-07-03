using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Visitors;

namespace NHibernate.Linq.Functions
{
	public class LikeGenerator : IHqlGeneratorForMethod, IRuntimeMethodHqlGenerator
	{
		public IEnumerable<MethodInfo> SupportedMethods
		{
			get { throw new NotImplementedException(); }
		}

		public HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments,
		                            HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			return treeBuilder.Like(
				visitor.Visit(arguments[0]).AsExpression(),
				visitor.Visit(arguments[1]).AsExpression());
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
			       method.GetParameters().Length == 2 &&
			       method.DeclaringType != null &&
			       method.DeclaringType.FullName.EndsWith("SqlMethods");
		}

		public IHqlGeneratorForMethod GetMethodGenerator(MethodInfo method)
		{
			return this;
		}
	}


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
			var stringExpr = visitor.Visit(targetObject).AsExpression();
			var start = treeBuilder.Add(visitor.Visit(arguments[0]).AsExpression(), treeBuilder.Constant(1));

			if (arguments.Count == 1)
				return treeBuilder.MethodCall("substring", stringExpr, start);

			var length = visitor.Visit(arguments[1]).AsExpression();
			return treeBuilder.MethodCall("substring", stringExpr, start, length);
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
						visitor.Visit(targetObject).AsExpression());//,
						//treeBuilder.Constant(0));
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

	public class TrimGenerator : BaseHqlGeneratorForMethod
	{
		public TrimGenerator()
		{
			SupportedMethods = new[]
									{
										ReflectionHelper.GetMethodDefinition<string>(s => s.Trim()),
										ReflectionHelper.GetMethodDefinition<string>(s => s.Trim('a')),
										ReflectionHelper.GetMethodDefinition<string>(s => s.TrimStart('a')),
										ReflectionHelper.GetMethodDefinition<string>(s => s.TrimEnd('a'))
									};
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

			string trimChars = "";
			if (method.GetParameters().Length > 0)
				foreach (char c in (char[])((ConstantExpression)arguments[0]).Value)
					trimChars += c;


			if (trimChars == "")
			{
				return treeBuilder.MethodCall("trim", treeBuilder.Ident(trimWhere), treeBuilder.Ident("from"), visitor.Visit(targetObject).AsExpression());
			}
			else
			{
				return treeBuilder.MethodCall("trim", treeBuilder.Ident(trimWhere), treeBuilder.Constant(trimChars), treeBuilder.Ident("from"), visitor.Visit(targetObject).AsExpression());
			}
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
		public IEnumerable<MethodInfo> SupportedMethods
		{
			get { throw new NotSupportedException(); }
		}

		public HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			return treeBuilder.MethodCall("str", visitor.Visit(targetObject).AsExpression());
		}
	}
}