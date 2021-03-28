using System.Collections.Generic;
using NHibernate.Cfg.ConfigurationSchema;

namespace NHibernate.Cfg
{
	public class SessionFactoryConfigurationBase : ISessionFactoryConfiguration
	{
		private string name = string.Empty;
		private readonly Dictionary<string, string> properties = new Dictionary<string, string>();
		private readonly List<MappingConfiguration> mappings = new List<MappingConfiguration>();
		private readonly List<ClassCacheConfiguration> classesCache= new List<ClassCacheConfiguration>();
		private readonly List<CollectionCacheConfiguration> collectionsCache= new List<CollectionCacheConfiguration>();
		private readonly List<EventConfiguration> events= new List<EventConfiguration>();
		private readonly List<ListenerConfiguration> listeners= new List<ListenerConfiguration>();

		/// <summary>
		/// The session factory name.
		/// </summary>
		public string Name
		{
			get { return name; }
			protected set { name = value; }
		}

		/// <summary>
		/// Session factory properties bag.
		/// </summary>
		public IDictionary<string,string> Properties
		{
			get { return properties; }
		}

		/// <summary>
		/// Session factory mapping configuration.
		/// </summary>
		public IList<MappingConfiguration> Mappings
		{
			get { return mappings; }
		}

		/// <summary>
		/// Session factory class-cache configurations.
		/// </summary>
		public IList<ClassCacheConfiguration> ClassesCache
		{
			get { return classesCache; }
		}

		/// <summary>
		/// Session factory collection-cache configurations.
		/// </summary>
		public IList<CollectionCacheConfiguration> CollectionsCache
		{
			get { return collectionsCache; }
		}

		/// <summary>
		/// Session factory event configurations.
		/// </summary>
		public IList<EventConfiguration> Events
		{
			get { return events; }
		}

		/// <summary>
		/// Session factory listener configurations.
		/// </summary>
		public IList<ListenerConfiguration> Listeners
		{
			get { return listeners; }
		}
	}
}
