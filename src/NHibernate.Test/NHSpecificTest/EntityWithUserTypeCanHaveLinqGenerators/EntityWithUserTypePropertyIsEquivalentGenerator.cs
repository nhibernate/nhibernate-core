using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Functions;
using NHibernate.Linq.Visitors;
using NHibernate.Util;

namespace NHibernate.Test.NHSpecificTest.EntityWithUserTypeCanHaveLinqGenerators
{
	public class EntityWithUserTypePropertyIsEquivalentGenerator : BaseHqlGeneratorForMethod
	{
		public EntityWithUserTypePropertyIsEquivalentGenerator()
		{
			SupportedMethods = new[] {ReflectHelper.GetMethodDefinition((IExample e) => e.IsEquivalentTo(null))};
		}

		public override HqlTreeNode BuildHql(
			MethodInfo method,
			Expression targetObject,
			ReadOnlyCollection<Expression> arguments,
			HqlTreeBuilder treeBuilder,
			IHqlExpressionVisitor visitor)
		{
			var left = treeBuilder.Cast(visitor.Visit(targetObject).AsExpression(), typeof(string));
			var right = treeBuilder.Cast(visitor.Visit(arguments.First()).AsExpression(), typeof(string));

			var leftSubstring = treeBuilder.MethodCall("substring", left, treeBuilder.Constant(4));
			var rightSubstring = treeBuilder.MethodCall("substring", right, treeBuilder.Constant(4));
			var equals = treeBuilder.Equality(leftSubstring, rightSubstring);
			return equals;
		}
	}
}
