using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Visitors;
using System.Linq;
using NHibernate.Util;

namespace NHibernate.Linq.Functions
{
	internal class CompareGenerator : BaseHqlGeneratorForMethod, IRuntimeMethodHqlGenerator
	{
		private static readonly MethodInfo MethodWithComparer = ReflectHelper.FastGetMethod(string.Compare, default(string), default(string), default(StringComparison));

		private static readonly HashSet<MethodInfo> ActingMethods = new HashSet<MethodInfo>
			{
				ReflectHelper.FastGetMethod(string.Compare, default(string), default(string)),
				MethodWithComparer,
				ReflectHelper.GetMethodDefinition<string>(s => s.CompareTo(s)),
				ReflectHelper.GetMethodDefinition<char>(x => x.CompareTo(x)),

				ReflectHelper.GetMethodDefinition<byte>(x => x.CompareTo(x)),
				ReflectHelper.GetMethodDefinition<sbyte>(x => x.CompareTo(x)),
				
				ReflectHelper.GetMethodDefinition<short>(x => x.CompareTo(x)),
				ReflectHelper.GetMethodDefinition<ushort>(x => x.CompareTo(x)),

				ReflectHelper.GetMethodDefinition<int>(x => x.CompareTo(x)),
				ReflectHelper.GetMethodDefinition<uint>(x => x.CompareTo(x)),

				ReflectHelper.GetMethodDefinition<long>(x => x.CompareTo(x)),
				ReflectHelper.GetMethodDefinition<ulong>(x => x.CompareTo(x)),

				ReflectHelper.GetMethodDefinition<float>(x => x.CompareTo(x)),
				ReflectHelper.GetMethodDefinition<double>(x => x.CompareTo(x)),
				
				ReflectHelper.FastGetMethod(decimal.Compare, default(decimal), default(decimal)),
				ReflectHelper.GetMethodDefinition<decimal>(x => x.CompareTo(x)),

				ReflectHelper.GetMethodDefinition<DateTime>(x => x.CompareTo(x)),
				ReflectHelper.GetMethodDefinition<DateTimeOffset>(x => x.CompareTo(x)),
			};

		internal static bool IsCompareMethod(MethodInfo methodInfo)
		{
			if (ActingMethods.Contains(methodInfo))
			{
				LogIgnoredStringComparisonParameter(methodInfo, MethodWithComparer);
				return true;
			}

			// This is .Net 4 only, and in the System.Data.Services assembly, which we don't depend directly on.
			return methodInfo != null && methodInfo.Name == "Compare" &&
				   methodInfo.DeclaringType != null &&
				   methodInfo.DeclaringType.FullName == "System.Data.Services.Providers.DataServiceProviderMethods";
		}

		public override bool AllowsNullableReturnType(MethodInfo method) => false;
		public CompareGenerator()
		{
			SupportedMethods = ActingMethods.ToArray();
		}

		public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			// Instance a.CompareTo(b) or static string.Compare(a, b)?
			Expression lhs = arguments.Count == 1 ? targetObject : arguments[0];
			Expression rhs = arguments.Count == 1 ? arguments[0] : arguments[1];

			HqlExpression lhs1 = visitor.Visit(lhs).AsExpression();
			HqlExpression rhs1 = visitor.Visit(rhs).AsExpression();
			HqlExpression lhs2 = visitor.Visit(lhs).AsExpression();
			HqlExpression rhs2 = visitor.Visit(rhs).AsExpression();

			// CASE WHEN (table.[Name] = N'Foo') THEN 0
			//      WHEN (table.[Name] > N'Foo') THEN 1
			//      ELSE -1 END

			return treeBuilder.Case(
				new[]
					{
						treeBuilder.When(treeBuilder.Equality(lhs1, rhs1), treeBuilder.Constant(0)),
						treeBuilder.When(treeBuilder.GreaterThan(lhs2, rhs2), treeBuilder.Constant(1))
					},
				treeBuilder.Constant(-1));
		}

		#region IRuntimeMethodHqlGenerator methods

		public bool SupportsMethod(MethodInfo method)
		{
			// SupportsMethod() is from IRuntimeMethodHqlGenerator. Strictly speaking we would only
			// need to handle the DataServiceProviderMethods.Compare() here (since the others
			// are registered from BaseHqlGeneratorForMethod.SupportedMethods), but for consistency
			// let's just delegate to the static IsCompareMethod().

			return IsCompareMethod(method);
		}

		public IHqlGeneratorForMethod GetMethodGenerator(MethodInfo method)
		{
			return this;
		}

		#endregion
	}
}
