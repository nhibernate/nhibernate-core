using System;
using System.Collections.Generic;

using NHibernate.Util;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Linq.Functions
{
	public sealed class LinqToHqlGeneratorsRegistryFactory
	{
		private static readonly INHibernateLogger log = NHibernateLogger.For(typeof (LinqToHqlGeneratorsRegistryFactory));

		public static ILinqToHqlGeneratorsRegistry CreateGeneratorsRegistry(IDictionary<string, string> properties)
		{
			if (properties.TryGetValue(Environment.LinqToHqlGeneratorsRegistry, out var registry))
			{
				try
				{
					log.Info("Initializing LinqToHqlGeneratorsRegistry: {0}", registry);
					return (ILinqToHqlGeneratorsRegistry) Environment.ServiceProvider.GetInstance(ReflectHelper.ClassForName(registry));
				}
				catch (Exception e)
				{
					log.Fatal(e, "Could not instantiate LinqToHqlGeneratorsRegistry");
					throw new HibernateException("Could not instantiate LinqToHqlGeneratorsRegistry: " + registry, e);
				}
			}
			try
			{
				return (ILinqToHqlGeneratorsRegistry)
				       Environment.ServiceProvider.GetService(typeof(ILinqToHqlGeneratorsRegistry)) ??
				       new DefaultLinqToHqlGeneratorsRegistry();
			}
			catch (Exception e)
			{
				log.Fatal(e, "Could not instantiate LinqToHqlGeneratorsRegistry");
				throw new HibernateException($"Could not instantiate LinqToHqlGeneratorsRegistry: {typeof(ILinqToHqlGeneratorsRegistry)}", e);
			}
		}
	}
}
