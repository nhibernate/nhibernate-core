using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Functions;
using NHibernate.Linq.Visitors;
using NHibernate.Util;

namespace NHibernate.Test.NHSpecificTest.NH2869
{
	public class IsTrueInDbFalseInLocalGenerator : BaseHqlGeneratorForMethod
	{
		public IsTrueInDbFalseInLocalGenerator()
		{
			SupportedMethods = new[] { ReflectHelper.GetMethodDefinition(() => MyLinqExtensions.IsOneInDbZeroInLocal(null, null)) };
		}

		public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			return treeBuilder.Constant(1);
		}
	}
}
