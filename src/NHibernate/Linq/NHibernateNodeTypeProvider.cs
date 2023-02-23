using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Util;
using Remotion.Linq.EagerFetching.Parsing;
using Remotion.Linq.Parsing.Structure;
using Remotion.Linq.Parsing.Structure.NodeTypeProviders;

namespace NHibernate.Linq
{
	public class NHibernateNodeTypeProvider : INodeTypeProvider
	{
		private INodeTypeProvider defaultNodeTypeProvider;

		public NHibernateNodeTypeProvider()
		{
			var methodInfoRegistry = new MethodInfoBasedNodeTypeRegistry();

			methodInfoRegistry.Register(
				new[] { ReflectHelper.FastGetMethodDefinition(EagerFetchingExtensionMethods.Fetch, default(IQueryable<object>), default(Expression<Func<object, object>>)) },
				typeof(FetchOneExpressionNode));
			methodInfoRegistry.Register(
				new[] { ReflectHelper.FastGetMethodDefinition(EagerFetchingExtensionMethods.FetchLazyProperties, default(IQueryable<object>)) },
				typeof(FetchLazyPropertiesExpressionNode));
			methodInfoRegistry.Register(
				new[] { ReflectHelper.FastGetMethodDefinition(EagerFetchingExtensionMethods.FetchMany, default(IQueryable<object>), default(Expression<Func<object, IEnumerable<object>>>)) },
				typeof(FetchManyExpressionNode));
			methodInfoRegistry.Register(
				new[] { ReflectHelper.FastGetMethodDefinition(EagerFetchingExtensionMethods.ThenFetch, default(INhFetchRequest<object, object>), default(Expression<Func<object, object>>)) },
				typeof(ThenFetchOneExpressionNode));
			methodInfoRegistry.Register(
				new[] { ReflectHelper.FastGetMethodDefinition(EagerFetchingExtensionMethods.ThenFetchMany, default(INhFetchRequest<object, object>), default(Expression<Func<object, IEnumerable<object>>>)) },
				typeof(ThenFetchManyExpressionNode));
			methodInfoRegistry.Register(
				new[]
				{
					ReflectHelper.FastGetMethodDefinition(LinqExtensionMethods.WithLock, default(IQueryable<object>), default(LockMode)),
					ReflectHelper.FastGetMethodDefinition(LinqExtensionMethods.WithLock, default(IEnumerable<object>), default(LockMode))
				},
				typeof(LockExpressionNode));

			var nodeTypeProvider = ExpressionTreeParser.CreateDefaultNodeTypeProvider();
			nodeTypeProvider.InnerProviders.Add(methodInfoRegistry);
			defaultNodeTypeProvider = nodeTypeProvider;
		}

		public bool IsRegistered(MethodInfo method)
		{
			// Avoid Relinq turning IDictionary.Contains into ContainsResultOperator.  We do our own processing for that method.
			if (method.DeclaringType == typeof(IDictionary) && method.Name == "Contains")
				return false;

			return defaultNodeTypeProvider.IsRegistered(method);
		}

		public System.Type GetNodeType(MethodInfo method)
		{
			return defaultNodeTypeProvider.GetNodeType(method);
		}
	}
}
