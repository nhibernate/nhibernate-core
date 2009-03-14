using System.Collections;

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

		/// <summary>
		/// Creates a resulttransformer that will inject aliased values into instances
		/// of <paramref name="target"/> via property methods or fields.
		/// </summary>
		public static IResultTransformer AliasToBean(System.Type target)
		{
			return new AliasToBeanResultTransformer(target);
		}

		public static IResultTransformer AliasToBean<T>() where T: class
		{
			return AliasToBean(typeof (T));
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
