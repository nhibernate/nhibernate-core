using System;
using System.Linq;
using System.Reflection;
using NHibernate.Linq;

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
				ReflectionHelper.GetMethodDefinition(() => Enumerable.Aggregate<object>(null, null));
			internal static readonly MethodInfo AggregateWithSeedDefinition =
				ReflectionHelper.GetMethodDefinition(() => Enumerable.Aggregate<object, object>(null, null, null));
			internal static readonly MethodInfo AggregateWithSeedAndResultSelectorDefinition =
				ReflectionHelper.GetMethodDefinition(() => Enumerable.Aggregate<object, object, object>(null, null, null, null));

			internal static readonly MethodInfo CastDefinition =
				ReflectionHelper.GetMethodDefinition(() => Enumerable.Cast<object>(null));

			internal static readonly MethodInfo GroupByWithElementSelectorDefinition = ReflectionHelper.GetMethodDefinition(
				() => Enumerable.GroupBy<object, object, object>(null, null, (Func<object, object>)null));

			internal static readonly MethodInfo SelectDefinition =
				ReflectionHelper.GetMethodDefinition(() => Enumerable.Select<object, object>(null, (Func<object, object>)null));

			internal static readonly MethodInfo ToArrayDefinition =
				ReflectionHelper.GetMethodDefinition(() => Enumerable.ToArray<object>(null));

			internal static readonly MethodInfo ToListDefinition =
				ReflectionHelper.GetMethodDefinition(() => Enumerable.ToList<object>(null));
		}
	}
}