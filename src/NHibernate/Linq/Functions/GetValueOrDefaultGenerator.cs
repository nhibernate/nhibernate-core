using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Visitors;
using NHibernate.Util;

namespace NHibernate.Linq.Functions
{
	internal class GetValueOrDefaultGenerator : IHqlGeneratorForMethod, IRuntimeMethodHqlGenerator
	{
		public bool SupportsMethod(MethodInfo method)
		{
			return method != null && String.Equals(method.Name, "GetValueOrDefault", StringComparison.OrdinalIgnoreCase) && method.IsMethodOf(typeof (Nullable<>));
		}

		public IHqlGeneratorForMethod GetMethodGenerator(MethodInfo method)
		{
			return this;
		}

		public IEnumerable<MethodInfo> SupportedMethods
		{
			get { throw new NotSupportedException("This is an runtime method generator"); }
		}

		public HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			return treeBuilder.Coalesce(visitor.Visit(targetObject).AsExpression(), GetRhs(method, arguments, treeBuilder, visitor));
		}

		private static HqlExpression GetRhs(MethodInfo method, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			if (arguments.Count > 0)
				return visitor.Visit(arguments[0]).AsExpression();

			var returnType = method.ReturnType;
			var instance = returnType.IsValueType ? Activator.CreateInstance(returnType) : null;
			return treeBuilder.Constant(instance);
		}
	}
}
