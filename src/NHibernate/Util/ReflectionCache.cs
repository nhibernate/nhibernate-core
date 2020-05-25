using System;
using System.Collections;
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
				ReflectHelper.FastGetMethodDefinition(Enumerable.Aggregate, default(IEnumerable<object>), default(Func<object, object, object>));
			internal static readonly MethodInfo AggregateWithSeedDefinition =
				ReflectHelper.FastGetMethodDefinition(Enumerable.Aggregate, default(IEnumerable<object>), default(object), default(Func<object, object, object>));
			internal static readonly MethodInfo AggregateWithSeedAndResultSelectorDefinition =
				ReflectHelper.FastGetMethodDefinition(Enumerable.Aggregate, default(IEnumerable<object>), default(object), default(Func<object, object, object>), default(Func<object, object>));

			internal static readonly MethodInfo AllDefinition =
				ReflectHelper.FastGetMethodDefinition(Enumerable.All, default(IEnumerable<object>), default(Func<object, bool>));

			internal static readonly MethodInfo CastDefinition =
				ReflectHelper.FastGetMethodDefinition(Enumerable.Cast<object>, default(IEnumerable));

			internal static readonly MethodInfo GroupByWithElementSelectorDefinition = 
				ReflectHelper.FastGetMethodDefinition(Enumerable.GroupBy, default(IEnumerable<object>), default(Func<object, object>), default(Func<object, object>));

			internal static readonly MethodInfo MaxDefinition =
				ReflectHelper.FastGetMethodDefinition(Enumerable.Max, default(IEnumerable<object>));

			internal static readonly MethodInfo MinDefinition =
				ReflectHelper.FastGetMethodDefinition(Enumerable.Min, default(IEnumerable<object>));

			internal static readonly MethodInfo SelectDefinition =
				ReflectHelper.FastGetMethodDefinition(Enumerable.Select, default(IEnumerable<object>), default(Func<object, object>));

			internal static readonly MethodInfo SumOnInt =
				ReflectHelper.FastGetMethod(Enumerable.Sum, default(IEnumerable<int>));

			internal static readonly MethodInfo ToArrayDefinition =
				ReflectHelper.FastGetMethodDefinition(Enumerable.ToArray, default(IEnumerable<object>));

			internal static readonly MethodInfo ToListDefinition =
				ReflectHelper.FastGetMethodDefinition(Enumerable.ToList, default(IEnumerable<object>));
		}

		internal static class MethodBaseMethods
		{
			internal static readonly MethodInfo GetMethodFromHandle =
				ReflectHelper.FastGetMethod(MethodBase.GetMethodFromHandle, default(RuntimeMethodHandle));
			internal static readonly MethodInfo GetMethodFromHandleWithDeclaringType =
				ReflectHelper.FastGetMethod(MethodBase.GetMethodFromHandle, default(RuntimeMethodHandle), default(RuntimeTypeHandle));
		}

		internal static class QueryableMethods
		{
			internal static readonly MethodInfo SelectDefinition =
				ReflectHelper.FastGetMethodDefinition(Queryable.Select, default(IQueryable<object>), default(Expression<Func<object, object>>));
			internal static readonly MethodInfo SelectManyDefinition =
				ReflectHelper.FastGetMethodDefinition(
					Queryable.SelectMany,
					default(IQueryable<object>),
					default(Expression<Func<object, IEnumerable<object>>>),
					default(Expression<Func<object, object, object>>));

			internal static readonly MethodInfo GroupJoinDefinition =
				ReflectHelper.FastGetMethodDefinition(
					Queryable.GroupJoin,
					default(IQueryable<object>),
					default(IEnumerable<object>),
					default(Expression<Func<object, int>>),
					default(Expression<Func<object, int>>),
					default(Expression<Func<object, IEnumerable<object>, object>>));

			internal static readonly MethodInfo CountDefinition =
				ReflectHelper.FastGetMethodDefinition(Queryable.Count, default(IQueryable<object>));
			internal static readonly MethodInfo CountWithPredicateDefinition =
				ReflectHelper.FastGetMethodDefinition(Queryable.Count, default(IQueryable<object>), default(Expression<Func<object, bool>>));

			internal static readonly MethodInfo LongCountDefinition =
				ReflectHelper.FastGetMethodDefinition(Queryable.LongCount, default(IQueryable<object>));
			internal static readonly MethodInfo LongCountWithPredicateDefinition =
				ReflectHelper.FastGetMethodDefinition(Queryable.LongCount, default(IQueryable<object>), default(Expression<Func<object, bool>>));

			internal static readonly MethodInfo AnyDefinition =
				ReflectHelper.FastGetMethodDefinition(Queryable.Any, default(IQueryable<object>));
			internal static readonly MethodInfo AnyWithPredicateDefinition =
				ReflectHelper.FastGetMethodDefinition(Queryable.Any, default(IQueryable<object>), default(Expression<Func<object, bool>>));
			
			internal static readonly MethodInfo AllDefinition =
				ReflectHelper.FastGetMethodDefinition(Queryable.All, default(IQueryable<object>), default(Expression<Func<object, bool>>));

			internal static readonly MethodInfo FirstDefinition =
				ReflectHelper.FastGetMethodDefinition(Queryable.First, default(IQueryable<object>));
			internal static readonly MethodInfo FirstWithPredicateDefinition =
				ReflectHelper.FastGetMethodDefinition(Queryable.First, default(IQueryable<object>), default(Expression<Func<object, bool>>));

			internal static readonly MethodInfo FirstOrDefaultDefinition =
				ReflectHelper.FastGetMethodDefinition(Queryable.FirstOrDefault, default(IQueryable<object>));
			internal static readonly MethodInfo FirstOrDefaultWithPredicateDefinition =
				ReflectHelper.FastGetMethodDefinition(Queryable.FirstOrDefault, default(IQueryable<object>), default(Expression<Func<object, bool>>));

			internal static readonly MethodInfo SingleDefinition =
				ReflectHelper.FastGetMethodDefinition(Queryable.Single, default(IQueryable<object>));
			internal static readonly MethodInfo SingleWithPredicateDefinition =
				ReflectHelper.FastGetMethodDefinition(Queryable.Single, default(IQueryable<object>), default(Expression<Func<object, bool>>));

			internal static readonly MethodInfo SingleOrDefaultDefinition =
				ReflectHelper.FastGetMethodDefinition(Queryable.SingleOrDefault<object>, default(IQueryable<object>));
			internal static readonly MethodInfo SingleOrDefaultWithPredicateDefinition =
				ReflectHelper.FastGetMethodDefinition(Queryable.SingleOrDefault<object>, default(IQueryable<object>), default(Expression<Func<object, bool>>));

			internal static readonly MethodInfo MinDefinition =
				ReflectHelper.FastGetMethodDefinition(Queryable.Min<object>, default(IQueryable<object>));
			internal static readonly MethodInfo MinWithSelectorDefinition =
				ReflectHelper.FastGetMethodDefinition(Queryable.Min<object, object>, default(IQueryable<object>), default(Expression<Func<object, object>>));

			internal static readonly MethodInfo MaxDefinition =
				ReflectHelper.FastGetMethodDefinition(Queryable.Max<object>, default(IQueryable<object>));
			internal static readonly MethodInfo MaxWithSelectorDefinition =
				ReflectHelper.FastGetMethodDefinition(Queryable.Max<object, object>, default(IQueryable<object>), default(Expression<Func<object, object>>));

			internal static readonly MethodInfo SumOfInt =
				ReflectHelper.FastGetMethod(Queryable.Sum, default(IQueryable<int>));
			internal static readonly MethodInfo SumOfNullableInt =
				ReflectHelper.FastGetMethod(Queryable.Sum, default(IQueryable<int?>));
			internal static readonly MethodInfo SumOfLong =
				ReflectHelper.FastGetMethod(Queryable.Sum, default(IQueryable<long>));
			internal static readonly MethodInfo SumOfNullableLong =
				ReflectHelper.FastGetMethod(Queryable.Sum, default(IQueryable<long?>));
			internal static readonly MethodInfo SumOfFloat =
				ReflectHelper.FastGetMethod(Queryable.Sum, default(IQueryable<float>));
			internal static readonly MethodInfo SumOfNullableFloat =
				ReflectHelper.FastGetMethod(Queryable.Sum, default(IQueryable<float?>));
			internal static readonly MethodInfo SumOfDouble =
				ReflectHelper.FastGetMethod(Queryable.Sum, default(IQueryable<double>));
			internal static readonly MethodInfo SumOfNullableDouble =
				ReflectHelper.FastGetMethod(Queryable.Sum, default(IQueryable<double?>));
			internal static readonly MethodInfo SumOfDecimal =
				ReflectHelper.FastGetMethod(Queryable.Sum, default(IQueryable<decimal>));
			internal static readonly MethodInfo SumOfNullableDecimal =
				ReflectHelper.FastGetMethod(Queryable.Sum, default(IQueryable<decimal?>));

			internal static readonly MethodInfo SumWithSelectorOfIntDefinition =
				ReflectHelper.FastGetMethodDefinition(Queryable.Sum, default(IQueryable<object>), default(Expression<Func<object, int>>));
			internal static readonly MethodInfo SumWithSelectorOfNullableIntDefinition =
				ReflectHelper.FastGetMethodDefinition(Queryable.Sum, default(IQueryable<object>), default(Expression<Func<object, int?>>));
			internal static readonly MethodInfo SumWithSelectorOfLongDefinition =
				ReflectHelper.FastGetMethodDefinition(Queryable.Sum, default(IQueryable<object>), default(Expression<Func<object, long>>));
			internal static readonly MethodInfo SumWithSelectorOfNullableLongDefinition =
				ReflectHelper.FastGetMethodDefinition(Queryable.Sum, default(IQueryable<object>), default(Expression<Func<object, long?>>));
			internal static readonly MethodInfo SumWithSelectorOfFloatDefinition =
				ReflectHelper.FastGetMethodDefinition(Queryable.Sum, default(IQueryable<object>), default(Expression<Func<object, float>>));
			internal static readonly MethodInfo SumWithSelectorOfNullableFloatDefinition =
				ReflectHelper.FastGetMethodDefinition(Queryable.Sum, default(IQueryable<object>), default(Expression<Func<object, float?>>));
			internal static readonly MethodInfo SumWithSelectorOfDoubleDefinition =
				ReflectHelper.FastGetMethodDefinition(Queryable.Sum, default(IQueryable<object>), default(Expression<Func<object, double>>));
			internal static readonly MethodInfo SumWithSelectorOfNullableDoubleDefinition =
				ReflectHelper.FastGetMethodDefinition(Queryable.Sum, default(IQueryable<object>), default(Expression<Func<object, double?>>));
			internal static readonly MethodInfo SumWithSelectorOfDecimalDefinition =
				ReflectHelper.FastGetMethodDefinition(Queryable.Sum, default(IQueryable<object>), default(Expression<Func<object, decimal>>));
			internal static readonly MethodInfo SumWithSelectorOfNullableDecimalDefinition =
				ReflectHelper.FastGetMethodDefinition(Queryable.Sum, default(IQueryable<object>), default(Expression<Func<object, decimal?>>));

			internal static readonly MethodInfo AverageOfInt =
				ReflectHelper.FastGetMethod(Queryable.Average, default(IQueryable<int>));
			internal static readonly MethodInfo AverageOfNullableInt =
				ReflectHelper.FastGetMethod(Queryable.Average, default(IQueryable<int?>));
			internal static readonly MethodInfo AverageOfLong =
				ReflectHelper.FastGetMethod(Queryable.Average, default(IQueryable<long>));
			internal static readonly MethodInfo AverageOfNullableLong =
				ReflectHelper.FastGetMethod(Queryable.Average, default(IQueryable<long?>));
			internal static readonly MethodInfo AverageOfFloat =
				ReflectHelper.FastGetMethod(Queryable.Average, default(IQueryable<float>));
			internal static readonly MethodInfo AverageOfNullableFloat =
				ReflectHelper.FastGetMethod(Queryable.Average, default(IQueryable<float?>));
			internal static readonly MethodInfo AverageOfDouble =
				ReflectHelper.FastGetMethod(Queryable.Average, default(IQueryable<double>));
			internal static readonly MethodInfo AverageOfNullableDouble =
				ReflectHelper.FastGetMethod(Queryable.Average, default(IQueryable<double?>));
			internal static readonly MethodInfo AverageOfDecimal =
				ReflectHelper.FastGetMethod(Queryable.Average, default(IQueryable<decimal>));
			internal static readonly MethodInfo AverageOfNullableDecimal =
				ReflectHelper.FastGetMethod(Queryable.Average, default(IQueryable<decimal?>));

			internal static readonly MethodInfo AverageWithSelectorOfIntDefinition =
				ReflectHelper.FastGetMethodDefinition(Queryable.Average, default(IQueryable<object>), default(Expression<Func<object, int>>));
			internal static readonly MethodInfo AverageWithSelectorOfNullableIntDefinition =
				ReflectHelper.FastGetMethodDefinition(Queryable.Average, default(IQueryable<object>), default(Expression<Func<object, int?>>));
			internal static readonly MethodInfo AverageWithSelectorOfLongDefinition =
				ReflectHelper.FastGetMethodDefinition(Queryable.Average, default(IQueryable<object>), default(Expression<Func<object, long>>));
			internal static readonly MethodInfo AverageWithSelectorOfNullableLongDefinition =
				ReflectHelper.FastGetMethodDefinition(Queryable.Average, default(IQueryable<object>), default(Expression<Func<object, long?>>));
			internal static readonly MethodInfo AverageWithSelectorOfFloatDefinition =
				ReflectHelper.FastGetMethodDefinition(Queryable.Average, default(IQueryable<object>), default(Expression<Func<object, float>>));
			internal static readonly MethodInfo AverageWithSelectorOfNullableFloatDefinition =
				ReflectHelper.FastGetMethodDefinition(Queryable.Average, default(IQueryable<object>), default(Expression<Func<object, float?>>));
			internal static readonly MethodInfo AverageWithSelectorOfDoubleDefinition =
				ReflectHelper.FastGetMethodDefinition(Queryable.Average, default(IQueryable<object>), default(Expression<Func<object, double>>));
			internal static readonly MethodInfo AverageWithSelectorOfNullableDoubleDefinition =
				ReflectHelper.FastGetMethodDefinition(Queryable.Average, default(IQueryable<object>), default(Expression<Func<object, double?>>));
			internal static readonly MethodInfo AverageWithSelectorOfDecimalDefinition =
				ReflectHelper.FastGetMethodDefinition(Queryable.Average, default(IQueryable<object>), default(Expression<Func<object, decimal>>));
			internal static readonly MethodInfo AverageWithSelectorOfNullableDecimalDefinition =
				ReflectHelper.FastGetMethodDefinition(Queryable.Average, default(IQueryable<object>), default(Expression<Func<object, decimal?>>));
		}

		internal static class TypeMethods
		{
			internal static readonly MethodInfo GetTypeFromHandle =
				ReflectHelper.FastGetMethod(System.Type.GetTypeFromHandle, default(RuntimeTypeHandle));
		}

		internal static class StringMethods
		{
			public static readonly MethodInfo EndsWith = ReflectHelper.GetMethodDefinition<string>(x => x.EndsWith(null));
			public static readonly MethodInfo StartsWith = ReflectHelper.GetMethodDefinition<string>(x => x.StartsWith(null));
			public static readonly MethodInfo Contains = ReflectHelper.GetMethodDefinition<string>(x => x.Contains(null));
		}
	}
}
