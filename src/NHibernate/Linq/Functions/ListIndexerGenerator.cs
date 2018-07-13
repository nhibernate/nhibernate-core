using System.Collections;
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
	internal class ListIndexerGenerator : BaseHqlGeneratorForMethod,IRuntimeMethodHqlGenerator
	{
		public ListIndexerGenerator()
		{
			SupportedMethods = new[]
			{
				ReflectHelper.GetMethodDefinition(() => Enumerable.ElementAt<object>(null, 0)),
				ReflectHelper.GetMethodDefinition(() => Queryable.ElementAt<object>(null, 0))
			};
		}

		public bool SupportsMethod(MethodInfo method)
		{
			return method != null &&
			       method.Name == "get_Item" &&
			       (method.IsMethodOf(typeof(IList)) || method.IsMethodOf(typeof(IList<>)));
		}

		public IHqlGeneratorForMethod GetMethodGenerator(MethodInfo method)
		{
			return this;
		}

		public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			var collection = visitor.Visit(method.IsStatic ? arguments[0] : targetObject).AsExpression();
			var index = visitor.Visit(method.IsStatic ? arguments[1] : arguments[0]).AsExpression();

			return treeBuilder.Index(collection, index);
		}
	}
}