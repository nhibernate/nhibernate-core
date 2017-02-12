using System.Collections.Generic;
using NHibernate.Cfg.ConfigurationSchema;

namespace NHibernate.Cfg
{
	public interface ISessionFactoryConfiguration
	{
		/// <summary>
		/// The session factory name.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Session factory properties bag.
		/// </summary>
		IDictionary<string, string> Properties { get; }

		/// <summary>
		/// Session factory mapping configuration.
		/// </summary>
		IList<MappingConfiguration> Mappings { get; }

		/// <summary>
		/// Session factory class-cache configurations.
		/// </summary>
		IList<ClassCacheConfiguration> ClassesCache { get; }

		/// <summary>
		/// Session factory collection-cache configurations.
		/// </summary>
		IList<CollectionCacheConfiguration> CollectionsCache { get; }

		/// <summary>
		/// Session factory event configurations.
		/// </summary>
		IList<EventConfiguration> Events { get; }

		/// <summary>
		/// Session factory listener configurations.
		/// </summary>
		IList<ListenerConfiguration> Listeners { get; }
	}
}