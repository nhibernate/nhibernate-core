using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Visitors;
using NHibernate.Util;
using System.Linq;

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
		public StartsWithGenerator()
		{
			SupportedMethods = new[] { ReflectHelper.GetMethodDefinition<string>(x => x.StartsWith(null)) };
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
			SupportedMethods = new[] { ReflectHelper.GetMethodDefinition<string>(x => x.EndsWith(null)) };
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
			SupportedMethods = new[] { ReflectHelper.GetMethodDefinition<string>(x => x.Contains(null)) };
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

	public class IndexOfGenerator : BaseHqlGeneratorForMethod
	{
		public IndexOfGenerator()
		{
			SupportedMethods = new[]
									{
										ReflectHelper.GetMethodDefinition<string>(s => s.IndexOf(' ')),
										ReflectHelper.GetMethodDefinition<string>(s => s.IndexOf(" ")),
										ReflectHelper.GetMethodDefinition<string>(s => s.IndexOf(' ', 0)),
										ReflectHelper.GetMethodDefinition<string>(s => s.IndexOf(" ", 0))
									};
		}
		public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			HqlMethodCall locate;
			if (arguments.Count == 1)
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
		public ReplaceGenerator()
		{
			SupportedMethods = new[]
									{
										ReflectHelper.GetMethodDefinition<string>(s => s.Replace(' ', ' ')),
										ReflectHelper.GetMethodDefinition<string>(s => s.Replace("", ""))
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

			string trimChars = "";
			if (method.GetParameters().Length > 0)
			{
				object argumentValue = ((ConstantExpression)arguments[0]).Value;
				if (argumentValue is char)
				{
					trimChars += (char)argumentValue;
				}
				else
				{
					foreach (char c in (char[])argumentValue)
					{
						trimChars += c;
					}
				}
			}


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
