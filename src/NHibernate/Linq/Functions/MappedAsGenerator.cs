using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Hql.Ast;
using NHibernate.Linq;
using NHibernate.Linq.Visitors;
using NHibernate.Type;

namespace NHibernate.Linq.Functions
{
	public class MappedAsGenerator : BaseHqlGeneratorForMethod
	{
		public MappedAsGenerator()
		{
			SupportedMethods = new[] { ReflectionHelper.GetMethodDefinition<object>(x => x.MappedAs(null)) };
		}

		public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			var result = visitor.Visit(arguments[0]).AsExpression();
			return result;
		}
	}
}
