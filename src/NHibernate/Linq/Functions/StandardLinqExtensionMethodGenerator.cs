using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Visitors;

namespace NHibernate.Linq.Functions
{
	public class StandardLinqExtensionMethodGenerator : IRuntimeMethodHqlGenerator
	{
		#region IRuntimeMethodHqlGenerator Members

		public bool SupportsMethod(MethodInfo method)
		{
			return method.GetCustomAttributes(typeof (LinqExtensionMethodAttribute), false).Any();
		}

		public IHqlGeneratorForMethod GetMethodGenerator(MethodInfo method)
		{
			return new HqlGeneratorForExtensionMethod((LinqExtensionMethodAttribute) method.GetCustomAttributes(typeof (LinqExtensionMethodAttribute), false).First(), method);
		}

		#endregion
	}

	public class HqlGeneratorForExtensionMethod : BaseHqlGeneratorForMethod
	{
		private readonly string _name;

		public HqlGeneratorForExtensionMethod(LinqExtensionMethodAttribute attribute, MethodInfo method)
		{
			_name = string.IsNullOrEmpty(attribute.Name) ? method.Name : attribute.Name;
		}

		public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			IEnumerable<HqlExpression> args = visitor.Visit(targetObject)
				.Union(arguments.Select(a => visitor.Visit(a)))
				.Cast<HqlExpression>();

			return treeBuilder.MethodCall(_name, args);
		}
	}

	internal static class UnionExtension
	{
		public static IEnumerable<HqlTreeNode> Union(this HqlTreeNode first, IEnumerable<HqlTreeNode> rest)
		{
			yield return first;

			foreach (HqlTreeNode x in rest)
			{
				yield return x;
			}
		}
	}
}