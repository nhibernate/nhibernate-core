using System;
using System.Collections.Generic;
using System.Linq;
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

		internal static class TypeMethods
		{
			internal static readonly MethodInfo GetTypeFromHandle =
				ReflectHelper.GetMethod(() => System.Type.GetTypeFromHandle(default(RuntimeTypeHandle)));
		}
	}
}