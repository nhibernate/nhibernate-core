using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Visitors;

namespace NHibernate.Linq.Functions
{
	public class CompareGenerator : BaseHqlGeneratorForMethod
	{
		public CompareGenerator()
		{
			SupportedMethods = new[]
				{
					ReflectionHelper.GetMethodDefinition(() => string.Compare(null, null)),
					ReflectionHelper.GetMethodDefinition<string>(s => s.CompareTo(s)),
					ReflectionHelper.GetMethodDefinition<char>(x => x.CompareTo(x)),
					ReflectionHelper.GetMethodDefinition<byte>(x => x.CompareTo(x)),
					ReflectionHelper.GetMethodDefinition<short>(x => x.CompareTo(x)),
					ReflectionHelper.GetMethodDefinition<int>(x => x.CompareTo(x)),
					ReflectionHelper.GetMethodDefinition<long>(x => x.CompareTo(x)),
					ReflectionHelper.GetMethodDefinition<float>(x => x.CompareTo(x)),
					ReflectionHelper.GetMethodDefinition<double>(x => x.CompareTo(x)),
					ReflectionHelper.GetMethodDefinition<decimal>(x => x.CompareTo(x)),
				};
		}

		public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			// Instance CompareTo() or static string.Compare()?
			Expression lhs = arguments.Count == 1 ? targetObject : arguments[0];
			Expression rhs = arguments.Count == 1 ? arguments[0] : arguments[1];

			HqlExpression lhs1 = visitor.Visit(lhs).AsExpression();
			HqlExpression rhs1 = visitor.Visit(rhs).AsExpression();
			HqlExpression lhs2 = visitor.Visit(lhs).AsExpression();
			HqlExpression rhs2 = visitor.Visit(rhs).AsExpression();

			// (CASE WHEN (table.[Name] = N'Foo')
			//       THEN 0
			//       ELSE CASE WHEN (table.[Name] > N'Foo')
			//                 THEN 1
			//                 ELSE -1 END END)

			return treeBuilder.Case(new[] { treeBuilder.When(treeBuilder.Equality(lhs1, rhs1), treeBuilder.Constant(0)) },
			                        treeBuilder.Case(
				                        new[] { treeBuilder.When(treeBuilder.GreaterThan(lhs2, rhs2), treeBuilder.Constant(1)) },
				                        treeBuilder.Constant(-1)));
		}
	}
}