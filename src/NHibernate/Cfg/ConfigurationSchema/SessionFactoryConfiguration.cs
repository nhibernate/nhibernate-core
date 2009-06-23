using System;
using System.Collections.Generic;
using System.Xml.XPath;

namespace NHibernate.Cfg.ConfigurationSchema
{
	/// <summary>
	/// Configuration parsed values for a session-factory XML node.
	/// </summary>
	public class SessionFactoryConfiguration : ISessionFactoryConfiguration
	{
		//private static readonly ILog log = LogManager.GetLogger(typeof(SessionFactoryConfiguration));

		internal SessionFactoryConfiguration(XPathNavigator hbConfigurationSection)
		{
			if (hbConfigurationSection == null)
				throw new ArgumentNullException("hbConfigurationSection");
			Parse(hbConfigurationSection);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SessionFactoryConfiguration"/> class.
		/// </summary>
		/// <param name="name">The session factory name. Null or empty string are allowed.</param>
		public SessionFactoryConfiguration(string name)
		{
			this.name = name;
		}

		private void Parse(XPathNavigator navigator)
		{
			ParseName(navigator);
			ParseProperties(navigator);
			ParseMappings(navigator);
			ParseClassesCache(navigator);
			ParseCollectionsCache(navigator);
			ParseListeners(navigator);
			ParseEvents(navigator);
		}

		private void ParseName(XPathNavigator navigator)
		{
			XPathNavigator xpn = navigator.SelectSingleNode(CfgXmlHelper.SessionFactoryExpression);
			if (xpn != null)
			{
				if (xpn.MoveToFirstAttribute())
					name = xpn.Value;
			}
		}

		private void ParseProperties(XPathNavigator navigator)
		{
			XPathNodeIterator xpni = navigator.Select(CfgXmlHelper.SessionFactoryPropertiesExpression);
			while (xpni.MoveNext())
			{
				string propName;
				string propValue;
				if(xpni.Current.Value!=null)
				{
					propValue = xpni.Current.Value.Trim();
				}
				else
				{
					propValue = string.Empty;
				}
				XPathNavigator pNav = xpni.Current.Clone();
				pNav.MoveToFirstAttribute();
				propName= pNav.Value;
				if (!string.IsNullOrEmpty(propName))
					properties[propName] = propValue;
			}
		}

		private void ParseMappings(XPathNavigator navigator)
		{
			XPathNodeIterator xpni = navigator.Select(CfgXmlHelper.SessionFactoryMappingsExpression);
			while (xpni.MoveNext())
			{
				MappingConfiguration mc = new MappingConfiguration(xpni.Current);
				if (!mc.IsEmpty())
				{
					// Workaround add first an assembly&resource and then only the same assembly. 
					// the <mapping> of whole assembly is ignored (included only sigles resources)
					// The "ignore" log, is enough ?
					// Perhaps we can add some intelligence to remove single resource reference when a whole assembly is referenced
					//if (!mappings.Contains(mc))
					//{
					//  mappings.Add(mc);
					//}
					//else
					//{
					//  string logMessage = "Ignored mapping -> " + mc.ToString();
					//  if (log.IsDebugEnabled)
					//    log.Debug(logMessage);
					//  if (log.IsWarnEnabled)
					//    log.Warn(logMessage);
					//}

					// The control to prevent mappings duplication was removed since the engine do the right thing 
					// for this issue (simple is better)
					mappings.Add(mc);
				}
			}
		}

		private void ParseClassesCache(XPathNavigator navigator)
		{
			XPathNodeIterator xpni = navigator.Select(CfgXmlHelper.SessionFactoryClassesCacheExpression);
			while (xpni.MoveNext())
			{
				classesCache.Add(new ClassCacheConfiguration(xpni.Current));
			}
		}


		private void ParseCollectionsCache(XPathNavigator navigator)
		{
			XPathNodeIterator xpni = navigator.Select(CfgXmlHelper.SessionFactoryCollectionsCacheExpression);
			while (xpni.MoveNext())
			{
				collectionsCache.Add(new CollectionCacheConfiguration(xpni.Current));
			}
		}


		private void ParseListeners(XPathNavigator navigator)
		{
			XPathNodeIterator xpni = navigator.Select(CfgXmlHelper.SessionFactoryListenersExpression);
			while (xpni.MoveNext())
			{
				listeners.Add(new ListenerConfiguration(xpni.Current));
			}
		}

		private void ParseEvents(XPathNavigator navigator)
		{
			XPathNodeIterator xpni = navigator.Select(CfgXmlHelper.SessionFactoryEventsExpression);
			while (xpni.MoveNext())
			{
				events.Add(new EventConfiguration(xpni.Current));
			}
		}

		private string name = string.Empty;
		/// <summary>
		/// The session factory name.
		/// </summary>
		public string Name
		{
			get { return name; }
		}

		private IDictionary<string, string> properties = new Dictionary<string, string>();
		/// <summary>
		/// Session factory propeties bag.
		/// </summary>
		public IDictionary<string,string> Properties
		{
			get { return properties; }
		}

		private IList<MappingConfiguration> mappings = new List<MappingConfiguration>();
		/// <summary>
		/// Session factory mapping configuration.
		/// </summary>
		public IList<MappingConfiguration> Mappings
		{
			get { return mappings; }
		}

		private IList<ClassCacheConfiguration> classesCache= new List<ClassCacheConfiguration>();
		/// <summary>
		/// Session factory class-cache configurations.
		/// </summary>
		public IList<ClassCacheConfiguration> ClassesCache
		{
			get { return classesCache; }
		}

		private IList<CollectionCacheConfiguration> collectionsCache= new List<CollectionCacheConfiguration>();
		/// <summary>
		/// Session factory collection-cache configurations.
		/// </summary>
		public IList<CollectionCacheConfiguration> CollectionsCache
		{
			get { return collectionsCache; }
		}

		private IList<EventConfiguration> events= new List<EventConfiguration>();
		/// <summary>
		/// Session factory event configurations.
		/// </summary>
		public IList<EventConfiguration> Events
		{
			get { return events; }
		}

		private IList<ListenerConfiguration> listeners= new List<ListenerConfiguration>();
		/// <summary>
		/// Session factory listener configurations.
		/// </summary>
		public IList<ListenerConfiguration> Listeners
		{
			get { return listeners; }
		}

	}
}
