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
			var instance = PropertiesHelper.GetInstance<ILinqToHqlGeneratorsRegistry>(
				Environment.LinqToHqlGeneratorsRegistry,
				properties,
				typeof(DefaultLinqToHqlGeneratorsRegistry));
			log.Info("LinqToHqlGeneratorsRegistry: '{0}'", instance.GetType().AssemblyQualifiedName);
			return instance;
		}
	}
}
