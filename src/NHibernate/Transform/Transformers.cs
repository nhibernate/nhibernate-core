using System.Collections;

namespace NHibernate.Transform
{
	public static class Transformers
	{
		/// <summary>
		/// Each row of results is a map (<see cref="IDictionary" />) from alias to values/entities
		/// </summary>
		public static readonly IResultTransformer AliasToEntityMap = AliasToEntityMapResultTransformer.Instance;

		/// <summary> Each row of results is a <see cref="IList"/></summary>
		public static readonly ToListResultTransformer ToList = ToListResultTransformer.Instance;

		/// <summary>
		/// Creates a result transformer that will inject aliased values into instances
		/// of <paramref name="target"/> via property methods or fields.
		/// </summary>
		/// <param name="target">The type of the instances to build.</param>
		/// <returns>A result transformer for supplied type.</returns>
		/// <remarks>
		/// Resolves setter for an alias with a heuristic: search among properties then fields for matching name and case, then,
		/// if no matching property or field was found, retry with a case insensitive match. For members having the same name, it
		/// sorts them by inheritance depth then by visibility from public to private, and takes those ranking first.
		/// </remarks>
		public static IResultTransformer AliasToBean(System.Type target)
		{
			return new AliasToBeanResultTransformer(target);
		}

		/// <summary>
		/// Creates a result transformer that will inject aliased values into instances
		/// of <paramref name="target"/> via property methods or fields.
		/// "Compiled" version of AliasToBean transformer. Performs better if you have many aliases and/or load many records.
		/// NOTE: This transformer can't be reused by different queries as it caches query aliases on first transformation
		/// </summary>
		/// <param name="target">The type of the instances to build.</param>
		/// <returns>A result transformer for supplied type.</returns>
		/// <remarks>
		/// Resolves setter for an alias with a heuristic: search among properties then fields for matching name and case, then,
		/// if no matching property or field was found, retry with a case insensitive match. For members having the same name, it
		/// sorts them by inheritance depth then by visibility from public to private, and takes those ranking first.
		/// </remarks>
		public static IResultTransformer AliasToBeanCompiled(System.Type target)
		{
			return new AliasToBeanCompiledResultTransformer(target);
		}

		/// <summary>
		/// Creates a result transformer that will inject aliased values into instances
		/// of <typeparamref name="T"/> via property methods or fields.
		/// </summary>
		/// <typeparam name="T">The type of the instances to build.</typeparam>
		/// <returns>A result transformer for supplied type.</returns>
		/// <remarks>
		/// Resolves setter for an alias with a heuristic: search among properties then fields for matching name and case, then,
		/// if no matching property or field was found, retry with a case insensitive match. For members having the same name, it
		/// sorts them by inheritance depth then by visibility from public to private, and takes those ranking first.
		/// </remarks>
		public static IResultTransformer AliasToBean<T>()
		{
			return AliasToBean(typeof(T));
		}

		/// <summary>
		/// Creates a result transformer that will inject aliased values into instances
		/// of <typeparamref name="T"/> via property methods or fields.
		/// "Compiled" version of AliasToBean transformer. Performs better if you have many aliases and/or load many records.
		/// NOTE: This transformer can't be reused by different queries as it caches query aliases on first transformation
		/// </summary>
		/// <typeparam name="T">The type of the instances to build.</typeparam>
		/// <returns>A result transformer for supplied type.</returns>
		/// <remarks>
		/// Resolves setter for an alias with a heuristic: search among properties then fields for matching name and case, then,
		/// if no matching property or field was found, retry with a case insensitive match. For members having the same name, it
		/// sorts them by inheritance depth then by visibility from public to private, and takes those ranking first.
		/// </remarks>
		public static IResultTransformer AliasToBeanCompiled<T>()
		{
			return AliasToBeanCompiled(typeof(T));
		}

		public static readonly IResultTransformer DistinctRootEntity = DistinctRootEntityResultTransformer.Instance;

		public static IResultTransformer AliasToBeanConstructor(System.Reflection.ConstructorInfo constructor)
		{
			return new AliasToBeanConstructorResultTransformer(constructor);
		}

		public static readonly IResultTransformer PassThrough = PassThroughResultTransformer.Instance;

		public static readonly IResultTransformer RootEntity = RootEntityResultTransformer.Instance;
	}
}
