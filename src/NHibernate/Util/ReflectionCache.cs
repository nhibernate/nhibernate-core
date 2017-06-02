using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NHibernate.Util
{
	internal static class ReflectionCache
	{
		// When adding a method to this cache, please follow the naming convention of those subclasses and fields:
		//  - Add your method to a subclass named according to the type holding the method, and suffixed with "Methods".
		//  - Name the field according to the method name.
		//  - If the method has overloads, suffix it with "With" followed by its parameter names. Do not list parameters
		//    common to all overloads.
		//  - If the method is a generic method definition, add "Definition" as final suffix.
		//  - If the method is generic, suffix it with "On" followed by its generic parameter type names.
		// Avoid caching here narrow cases, such as those using specific types and unlikely to be used by many classes.
		// Cache them instead in classes using them.

		internal static class EnumerableMethods
		{
			internal static readonly MethodInfo AggregateDefinition =
				ReflectHelper.GetMethodDefinition(() => Enumerable.Aggregate<object>(null, null));
			internal static readonly MethodInfo AggregateWithSeedDefinition =
				ReflectHelper.GetMethodDefinition(() => Enumerable.Aggregate<object, object>(null, null, null));
			internal static readonly MethodInfo AggregateWithSeedAndResultSelectorDefinition =
				ReflectHelper.GetMethodDefinition(() => Enumerable.Aggregate<object, object, object>(null, null, null, null));

			internal static readonly MethodInfo AllDefinition =
				ReflectHelper.GetMethodDefinition(() => Enumerable.All<object>(null, null));

			internal static readonly MethodInfo CastDefinition =
				ReflectHelper.GetMethodDefinition(() => Enumerable.Cast<object>(null));

			internal static readonly MethodInfo GroupByWithElementSelectorDefinition = ReflectHelper.GetMethodDefinition(
				() => Enumerable.GroupBy<object, object, object>(null, null, default(Func<object, object>)));

			internal static readonly MethodInfo MaxDefinition =
				ReflectHelper.GetMethodDefinition(() => Enumerable.Max<object>(null));

			internal static readonly MethodInfo MinDefinition =
				ReflectHelper.GetMethodDefinition(() => Enumerable.Min<object>(null));

			internal static readonly MethodInfo SelectDefinition =
				ReflectHelper.GetMethodDefinition(() => Enumerable.Select(null, default(Func<object, object>)));

			internal static readonly MethodInfo SumOnInt =
				ReflectHelper.GetMethod(() => Enumerable.Sum(default(IEnumerable<int>)));

			internal static readonly MethodInfo ToArrayDefinition =
				ReflectHelper.GetMethodDefinition(() => Enumerable.ToArray<object>(null));

			internal static readonly MethodInfo ToListDefinition =
				ReflectHelper.GetMethodDefinition(() => Enumerable.ToList<object>(null));
		}

		internal static class MethodBaseMethods
		{
			internal static readonly MethodInfo GetMethodFromHandle =
				ReflectHelper.GetMethod(() => MethodBase.GetMethodFromHandle(default(RuntimeMethodHandle)));
			internal static readonly MethodInfo GetMethodFromHandleWithDeclaringType =
				ReflectHelper.GetMethod(() => MethodBase.GetMethodFromHandle(default(RuntimeMethodHandle), default(RuntimeTypeHandle)));
		}

		internal static class QueryableMethods
		{
			internal static readonly MethodInfo CountDefinition =
				ReflectHelper.GetMethodDefinition(() => Queryable.Count<object>(null));
			internal static readonly MethodInfo CountWithPredicateDefinition =
				ReflectHelper.GetMethodDefinition(() => Queryable.Count<object>(null, null));

			internal static readonly MethodInfo LongCountDefinition =
				ReflectHelper.GetMethodDefinition(() => Queryable.LongCount<object>(null));
			internal static readonly MethodInfo LongCountWithPredicateDefinition =
				ReflectHelper.GetMethodDefinition(() => Queryable.LongCount<object>(null, null));

			internal static readonly MethodInfo AnyDefinition =
				ReflectHelper.GetMethodDefinition(() => Queryable.Any<object>(null));
			internal static readonly MethodInfo AnyWithPredicateDefinition =
				ReflectHelper.GetMethodDefinition(() => Queryable.Any<object>(null, null));
			
			internal static readonly MethodInfo AllDefinition =
				ReflectHelper.GetMethodDefinition(() => Queryable.All<object>(null, null));

			internal static readonly MethodInfo FirstDefinition =
				ReflectHelper.GetMethodDefinition(() => Queryable.First<object>(null));
			internal static readonly MethodInfo FirstWithPredicateDefinition =
				ReflectHelper.GetMethodDefinition(() => Queryable.First<object>(null, null));

			internal static readonly MethodInfo FirstOrDefaultDefinition =
				ReflectHelper.GetMethodDefinition(() => Queryable.FirstOrDefault<object>(null));
			internal static readonly MethodInfo FirstOrDefaultWithPredicateDefinition =
				ReflectHelper.GetMethodDefinition(() => Queryable.FirstOrDefault<object>(null, null));

			internal static readonly MethodInfo SingleDefinition =
				ReflectHelper.GetMethodDefinition(() => Queryable.Single<object>(null));
			internal static readonly MethodInfo SingleWithPredicateDefinition =
				ReflectHelper.GetMethodDefinition(() => Queryable.Single<object>(null, null));

			internal static readonly MethodInfo SingleOrDefaultDefinition =
				ReflectHelper.GetMethodDefinition(() => Queryable.SingleOrDefault<object>(null));
			internal static readonly MethodInfo SingleOrDefaultWithPredicateDefinition =
				ReflectHelper.GetMethodDefinition(() => Queryable.SingleOrDefault<object>(null, null));

			internal static readonly MethodInfo MinDefinition =
				ReflectHelper.GetMethodDefinition(() => Queryable.Min<object>(null));
			internal static readonly MethodInfo MinWithSelectorDefinition =
				ReflectHelper.GetMethodDefinition(() => Queryable.Min<object, object>(null, null));

			internal static readonly MethodInfo MaxDefinition =
				ReflectHelper.GetMethodDefinition(() => Queryable.Max<object>(null));
			internal static readonly MethodInfo MaxWithSelectorDefinition =
				ReflectHelper.GetMethodDefinition(() => Queryable.Max<object, object>(null, null));

			internal static readonly MethodInfo SumOfInt =
				ReflectHelper.GetMethod(() => Queryable.Sum(default(IQueryable<int>)));
			internal static readonly MethodInfo SumOfNullableInt =
				ReflectHelper.GetMethod(() => Queryable.Sum(default(IQueryable<int?>)));
			internal static readonly MethodInfo SumOfLong =
				ReflectHelper.GetMethod(() => Queryable.Sum(default(IQueryable<long>)));
			internal static readonly MethodInfo SumOfNullableLong =
				ReflectHelper.GetMethod(() => Queryable.Sum(default(IQueryable<long?>)));
			internal static readonly MethodInfo SumOfFloat =
				ReflectHelper.GetMethod(() => Queryable.Sum(default(IQueryable<float>)));
			internal static readonly MethodInfo SumOfNullableFloat =
				ReflectHelper.GetMethod(() => Queryable.Sum(default(IQueryable<float?>)));
			internal static readonly MethodInfo SumOfDouble =
				ReflectHelper.GetMethod(() => Queryable.Sum(default(IQueryable<double>)));
			internal static readonly MethodInfo SumOfNullableDouble =
				ReflectHelper.GetMethod(() => Queryable.Sum(default(IQueryable<double?>)));
			internal static readonly MethodInfo SumOfDecimal =
				ReflectHelper.GetMethod(() => Queryable.Sum(default(IQueryable<decimal>)));
			internal static readonly MethodInfo SumOfNullableDecimal =
				ReflectHelper.GetMethod(() => Queryable.Sum(default(IQueryable<decimal?>)));

			internal static readonly MethodInfo SumWithSelectorOfIntDefinition =
				ReflectHelper.GetMethodDefinition(() => Queryable.Sum(null, default(Expression<Func<object, int>>)));
			internal static readonly MethodInfo SumWithSelectorOfNullableIntDefinition =
				ReflectHelper.GetMethodDefinition(() => Queryable.Sum(null, default(Expression<Func<object, int?>>)));
			internal static readonly MethodInfo SumWithSelectorOfLongDefinition =
				ReflectHelper.GetMethodDefinition(() => Queryable.Sum(null, default(Expression<Func<object, long>>)));
			internal static readonly MethodInfo SumWithSelectorOfNullableLongDefinition =
				ReflectHelper.GetMethodDefinition(() => Queryable.Sum(null, default(Expression<Func<object, long?>>)));
			internal static readonly MethodInfo SumWithSelectorOfFloatDefinition =
				ReflectHelper.GetMethodDefinition(() => Queryable.Sum(null, default(Expression<Func<object, float>>)));
			internal static readonly MethodInfo SumWithSelectorOfNullableFloatDefinition =
				ReflectHelper.GetMethodDefinition(() => Queryable.Sum(null, default(Expression<Func<object, float?>>)));
			internal static readonly MethodInfo SumWithSelectorOfDoubleDefinition =
				ReflectHelper.GetMethodDefinition(() => Queryable.Sum(null, default(Expression<Func<object, double>>)));
			internal static readonly MethodInfo SumWithSelectorOfNullableDoubleDefinition =
				ReflectHelper.GetMethodDefinition(() => Queryable.Sum(null, default(Expression<Func<object, double?>>)));
			internal static readonly MethodInfo SumWithSelectorOfDecimalDefinition =
				ReflectHelper.GetMethodDefinition(() => Queryable.Sum(null, default(Expression<Func<object, decimal>>)));
			internal static readonly MethodInfo SumWithSelectorOfNullableDecimalDefinition =
				ReflectHelper.GetMethodDefinition(() => Queryable.Sum(null, default(Expression<Func<object, decimal?>>)));

			internal static readonly MethodInfo AverageOfInt =
				ReflectHelper.GetMethod(() => Queryable.Average(default(IQueryable<int>)));
			internal static readonly MethodInfo AverageOfNullableInt =
				ReflectHelper.GetMethod(() => Queryable.Average(default(IQueryable<int?>)));
			internal static readonly MethodInfo AverageOfLong =
				ReflectHelper.GetMethod(() => Queryable.Average(default(IQueryable<long>)));
			internal static readonly MethodInfo AverageOfNullableLong =
				ReflectHelper.GetMethod(() => Queryable.Average(default(IQueryable<long?>)));
			internal static readonly MethodInfo AverageOfFloat =
				ReflectHelper.GetMethod(() => Queryable.Average(default(IQueryable<float>)));
			internal static readonly MethodInfo AverageOfNullableFloat =
				ReflectHelper.GetMethod(() => Queryable.Average(default(IQueryable<float?>)));
			internal static readonly MethodInfo AverageOfDouble =
				ReflectHelper.GetMethod(() => Queryable.Average(default(IQueryable<double>)));
			internal static readonly MethodInfo AverageOfNullableDouble =
				ReflectHelper.GetMethod(() => Queryable.Average(default(IQueryable<double?>)));
			internal static readonly MethodInfo AverageOfDecimal =
				ReflectHelper.GetMethod(() => Queryable.Average(default(IQueryable<decimal>)));
			internal static readonly MethodInfo AverageOfNullableDecimal =
				ReflectHelper.GetMethod(() => Queryable.Average(default(IQueryable<decimal?>)));

			internal static readonly MethodInfo AverageWithSelectorOfIntDefinition =
				ReflectHelper.GetMethodDefinition(() => Queryable.Average(null, default(Expression<Func<object, int>>)));
			internal static readonly MethodInfo AverageWithSelectorOfNullableIntDefinition =
				ReflectHelper.GetMethodDefinition(() => Queryable.Average(null, default(Expression<Func<object, int?>>)));
			internal static readonly MethodInfo AverageWithSelectorOfLongDefinition =
				ReflectHelper.GetMethodDefinition(() => Queryable.Average(null, default(Expression<Func<object, long>>)));
			internal static readonly MethodInfo AverageWithSelectorOfNullableLongDefinition =
				ReflectHelper.GetMethodDefinition(() => Queryable.Average(null, default(Expression<Func<object, long?>>)));
			internal static readonly MethodInfo AverageWithSelectorOfFloatDefinition =
				ReflectHelper.GetMethodDefinition(() => Queryable.Average(null, default(Expression<Func<object, float>>)));
			internal static readonly MethodInfo AverageWithSelectorOfNullableFloatDefinition =
				ReflectHelper.GetMethodDefinition(() => Queryable.Average(null, default(Expression<Func<object, float?>>)));
			internal static readonly MethodInfo AverageWithSelectorOfDoubleDefinition =
				ReflectHelper.GetMethodDefinition(() => Queryable.Average(null, default(Expression<Func<object, double>>)));
			internal static readonly MethodInfo AverageWithSelectorOfNullableDoubleDefinition =
				ReflectHelper.GetMethodDefinition(() => Queryable.Average(null, default(Expression<Func<object, double?>>)));
			internal static readonly MethodInfo AverageWithSelectorOfDecimalDefinition =
				ReflectHelper.GetMethodDefinition(() => Queryable.Average(null, default(Expression<Func<object, decimal>>)));
			internal static readonly MethodInfo AverageWithSelectorOfNullableDecimalDefinition =
				ReflectHelper.GetMethodDefinition(() => Queryable.Average(null, default(Expression<Func<object, decimal?>>)));
		}

		internal static class TypeMethods
		{
			internal static readonly MethodInfo GetTypeFromHandle =
				ReflectHelper.GetMethod(() => System.Type.GetTypeFromHandle(default(RuntimeTypeHandle)));
		}
	}
}