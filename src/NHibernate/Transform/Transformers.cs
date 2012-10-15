using System.Collections;
using System.Collections.Generic;

namespace NHibernate.Transform
{
	public sealed class Transformers
	{
		private Transformers()
		{
		}

		/// <summary>
		/// Each row of results is a map (<see cref="IDictionary" />) from alias to values/entities
		/// </summary>
		public static readonly IResultTransformer AliasToEntityMap = new AliasToEntityMapResultTransformer();

		/// <summary> Each row of results is a <see cref="IList"/></summary>
		public static readonly ToListResultTransformer ToList = new ToListResultTransformer();

		private static readonly Dictionary<System.Type, AliasToBeanResultTransformer> aliasToBeanInstances = new Dictionary<System.Type, AliasToBeanResultTransformer>();

		/// <summary>
		/// Creates an IResultTransformer that will inject aliased values into instances
		/// of <paramref name="target"/> via property methods or fields.
		/// <param name="target">The class type used to transform the result set</param>
		/// </summary>
		public static IResultTransformer AliasToBean(System.Type target)
		{
			return AliasToBean(target, false);
		}

		
		/// <summary>
		/// Returns an IResultTransformer that will inject aliased values into instances
		/// of <paramref name="target"/> via property methods or fields.
		/// </summary>
		/// <param name="target">The class type used to transform the result set</param>
		/// <param name="useInstanceCache">Secifies whether to return a cached or new instance</param>	
		public static IResultTransformer AliasToBean(System.Type target, bool useInstanceCache)
		{
			AliasToBeanResultTransformer transformer = null;

			if (useInstanceCache)
			{
				if (aliasToBeanInstances.ContainsKey(target))
				{
					return aliasToBeanInstances[target];
				}

				transformer = new AliasToBeanResultTransformer(target);

				aliasToBeanInstances.Add(target, transformer);
			}

			return transformer ?? new AliasToBeanResultTransformer(target);
		}

		/// <summary>
		/// Creates an IResultTransformer that will inject aliased values into instances
		/// of <paramref name="target"/> via property methods or fields.
		/// <param name="target">The class type used to transform the result set</param>
		/// </summary>
		public static IResultTransformer AliasToBean<T>()
		{
			return AliasToBean(typeof(T));
		}

		/// <summary>
		/// Returns an IResultTransformer that will inject aliased values into instances
		/// of <paramref name="target"/> via property methods or fields.
		/// </summary>
		/// <param name="target">The class type used to transform the result set</param>
		/// <param name="useInstanceCache">Secifies whether to return a cached or new instance</param>	
		public static IResultTransformer AliasToBean<T>(bool useInstanceCache)
		{
			return AliasToBean(typeof(T), useInstanceCache);
		}

		public static readonly IResultTransformer DistinctRootEntity = new DistinctRootEntityResultTransformer();

		public static IResultTransformer AliasToBeanConstructor(System.Reflection.ConstructorInfo constructor)
		{
			return new AliasToBeanConstructorResultTransformer(constructor);
		}

		public static readonly IResultTransformer PassThrough = new PassThroughResultTransformer();

		public static readonly IResultTransformer RootEntity = new RootEntityResultTransformer();
	}
}
