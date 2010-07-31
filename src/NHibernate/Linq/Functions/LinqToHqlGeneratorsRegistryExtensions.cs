using System;
using System.Linq;

namespace NHibernate.Linq.Functions
{
	public static class LinqToHqlGeneratorsRegistryExtensions
	{
		public static void Merge(this ILinqToHqlGeneratorsRegistry registry, IHqlGeneratorForMethod generator)
		{
			if (registry == null)
			{
				throw new ArgumentNullException("registry");
			}
			if (generator == null)
			{
				throw new ArgumentNullException("generator");
			}
			Array.ForEach(generator.SupportedMethods.ToArray(), method=> registry.RegisterGenerator(method, generator));
		}

		public static void Merge(this ILinqToHqlGeneratorsRegistry registry, IHqlGeneratorForProperty generator)
		{
			if (registry == null)
			{
				throw new ArgumentNullException("registry");
			}
			if (generator == null)
			{
				throw new ArgumentNullException("generator");
			}
			Array.ForEach(generator.SupportedProperties.ToArray(), property => registry.RegisterGenerator(property, generator));
		}
	}
}