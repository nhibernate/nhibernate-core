using System;

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
			foreach (var method in generator.SupportedMethods)
			{
				registry.RegisterGenerator(method, generator);
			}
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
			foreach (var property in generator.SupportedProperties)
			{
				registry.RegisterGenerator(property, generator);
			}
		}
	}
}