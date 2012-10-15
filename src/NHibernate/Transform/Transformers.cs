using System.Collections;
using System.Collections.Generic;

namespace NHibernate.Transform
{
	public sealed class Transformers
	{
		private static readonly Dictionary<System.Type, AliasToBeanResultTransformer> aliasToBeanInstances = new Dictionary<System.Type, AliasToBeanResultTransformer>();

		private Transformers()
		{
		}

		/// <summary>
		/// Each row of results is a map (<see cref="IDictionary" />) from alias to values/entities
		/// </summary>
		public static readonly IResultTransformer AliasToEntityMap = new AliasToEntityMapResultTransformer();

		/// <summary> Each row of results is a <see cref="IList"/></summary>
		public static readonly ToListResultTransformer ToList = new ToListResultTransformer();

		/// <summary>
		/// Creates a resulttransformer that will inject aliased values into instances
		/// of <paramref name="target"/> via property methods or fields.
		/// </summary>
		public static IResultTransformer AliasToBean(System.Type target)
		{
			if (aliasToBeanInstances.ContainsKey(target))
			{
				return aliasToBeanInstances[target];
			}

			var transformer = new AliasToBeanResultTransformer(target);

			aliasToBeanInstances.Add(target, transformer);

			return transformer;
		}

		public static IResultTransformer AliasToBean<T>()
		{
			return AliasToBean(typeof(T));
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
