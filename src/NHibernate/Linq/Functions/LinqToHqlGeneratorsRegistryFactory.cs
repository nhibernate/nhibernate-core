using System;
using System.Collections.Generic;
using log4net;
using NHibernate.Util;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Linq.Functions
{
	public sealed class LinqToHqlGeneratorsRegistryFactory
	{
		private static readonly ILog log = LogManager.GetLogger(typeof (LinqToHqlGeneratorsRegistryFactory));

		public static ILinqToHqlGeneratorsRegistry CreateGeneratorsRegistry(IDictionary<string, string> properties)
		{
			string registry;
			if (properties.TryGetValue(Environment.LinqToHqlGeneratorsRegistry, out registry))
			{
				try
				{
					log.Info("Initializing LinqToHqlGeneratorsRegistry: " + registry);
					return (ILinqToHqlGeneratorsRegistry) Environment.BytecodeProvider.ObjectsFactory.CreateInstance(ReflectHelper.ClassForName(registry));
				}
				catch (Exception e)
				{
					log.Fatal("Could not instantiate LinqToHqlGeneratorsRegistry", e);
					throw new HibernateException("Could not instantiate LinqToHqlGeneratorsRegistry: " + registry, e);
				}
			}
			return new FunctionRegistry();
		}
	}
}