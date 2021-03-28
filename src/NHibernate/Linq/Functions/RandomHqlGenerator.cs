using System;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Engine;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Visitors;
using NHibernate.Util;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Linq.Functions
{
	public class RandomHqlGenerator : BaseHqlGeneratorForMethod, IAllowPreEvaluationHqlGenerator
	{
		private readonly MethodInfo _nextDouble = ReflectHelper.GetMethod<Random>(r => r.NextDouble());
		private const string _randomFunctionName = "random";
		private const string _floorFunctionName = "floor";

		public RandomHqlGenerator()
		{
			SupportedMethods = new[]
			{
				_nextDouble,
				ReflectHelper.GetMethod<Random>(r => r.Next()),
				ReflectHelper.GetMethod<Random>(r => r.Next(2)),
				ReflectHelper.GetMethod<Random>(r => r.Next(-1, 1))
			};
		}

		public override HqlTreeNode BuildHql(
			MethodInfo method,
			Expression targetObject,
			ReadOnlyCollection<Expression> arguments,
			HqlTreeBuilder treeBuilder,
			IHqlExpressionVisitor visitor)
		{
			if (method == _nextDouble)
				return treeBuilder.MethodCall(_randomFunctionName);

			switch (arguments.Count)
			{
				case 0:
					return treeBuilder.Cast(
						treeBuilder.MethodCall(
							_floorFunctionName,
							treeBuilder.Multiply(
								treeBuilder.MethodCall(_randomFunctionName),
								treeBuilder.Constant(int.MaxValue))),
						typeof(int));
				case 1:
					return treeBuilder.Cast(
						treeBuilder.MethodCall(
							_floorFunctionName,
							treeBuilder.Multiply(
								treeBuilder.MethodCall(_randomFunctionName),
								visitor.Visit(arguments[0]).AsExpression())),
						typeof(int));
				case 2:
					var minValue = visitor.Visit(arguments[0]).AsExpression();
					var maxValue = visitor.Visit(arguments[1]).AsExpression();
					return treeBuilder.Cast(
						treeBuilder.Add(
							treeBuilder.MethodCall(
								_floorFunctionName,
								treeBuilder.Multiply(
									treeBuilder.MethodCall(_randomFunctionName),
									treeBuilder.Subtract(maxValue, minValue))),
							minValue),
						typeof(int));
				default:
					throw new NotSupportedException();
			}
		}

		/// <inheritdoc />
		public bool AllowPreEvaluation(MemberInfo member, ISessionFactoryImplementor factory)
		{
			if (factory.Dialect.Functions.ContainsKey(_randomFunctionName) &&
				(member == _nextDouble || factory.Dialect.Functions.ContainsKey(_floorFunctionName)))
				return false;

			if (factory.Settings.LinqToHqlFallbackOnPreEvaluation)
				return true;

			var functionName = factory.Dialect.Functions.ContainsKey(_randomFunctionName)
				? _floorFunctionName
				: _randomFunctionName;
			throw new QueryException(
				$"Cannot translate {member.DeclaringType.Name}.{member.Name}: {functionName} is " +
				$"not supported by {factory.Dialect}. Either enable the fallback on pre-evaluation " +
				$"({Environment.LinqToHqlFallbackOnPreEvaluation}) or evaluate {member.Name} " +
				"outside of the query.");
		}

		/// <inheritdoc />
		public bool IgnoreInstance(MemberInfo member)
		{
			// The translation ignores the Random instance, so long if it was specifically seeded: the user should
			// pass the random value as a local variable in the Linq query in such case.
			// Returning false here would cause the method, when appearing in a select clause, to be post-evaluated.
			// Contrary to pre-evaluation, the post-evaluation is done for each row so it at least would avoid having
			// the same random value for each result.
			// But that would still be not executed in database which would be unexpected, in my opinion.
			// It would even cause failures if the random instance used for querying is shared among threads or is
			// too similarly seeded between queries.
			return true;
		}
	}
}
