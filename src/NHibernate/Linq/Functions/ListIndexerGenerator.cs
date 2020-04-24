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
		private static readonly HashSet<MethodInfo> _supportedMethods = new HashSet<MethodInfo>
		{
			ReflectHelper.FastGetMethodDefinition(Enumerable.ElementAt, default(IEnumerable<object>), 0),
			ReflectHelper.FastGetMethodDefinition(Queryable.ElementAt, default(IQueryable<object>), 0)
		};

		public ListIndexerGenerator()
		{
			SupportedMethods = _supportedMethods;
		}

		public bool SupportsMethod(MethodInfo method)
		{
			return IsRuntimeMethodSupported(method);
		}

		public static bool IsMethodSupported(MethodInfo method)
		{
			if (method.IsGenericMethod)
			{
				method = method.GetGenericMethodDefinition();
			}

			return _supportedMethods.Contains(method) || IsRuntimeMethodSupported(method);
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

		private static bool IsRuntimeMethodSupported(MethodInfo method)
		{
			return method != null &&
					method.Name == "get_Item" &&
					(method.IsMethodOf(typeof(IList)) || method.IsMethodOf(typeof(IList<>)));
		}
	}
}
