using System;
using System.Xml.XPath;

namespace NHibernate.Cfg.ConfigurationSchema
{
	/// <summary>
	/// Configuration parsed values for a session-factory XML node.
	/// </summary>
	public class SessionFactoryConfiguration : SessionFactoryConfigurationBase
	{
		//private static readonly ILogger log = LoggerProvider.LoggerFor(typeof(SessionFactoryConfiguration));

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
			Name = name;
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
					Name = xpn.Value;
			}
		}

		private void ParseProperties(XPathNavigator navigator)
		{
			XPathNodeIterator xpni = navigator.Select(CfgXmlHelper.SessionFactoryPropertiesExpression);
			while (xpni.MoveNext())
			{
				string propValue = xpni.Current.Value!=null ? xpni.Current.Value.Trim() : string.Empty;
				XPathNavigator pNav = xpni.Current.Clone();
				pNav.MoveToFirstAttribute();
				string propName = pNav.Value;
				if (!string.IsNullOrEmpty(propName))
					Properties[propName] = propValue;
			}
		}

		private void ParseMappings(XPathNavigator navigator)
		{
			XPathNodeIterator xpni = navigator.Select(CfgXmlHelper.SessionFactoryMappingsExpression);
			while (xpni.MoveNext())
			{
				var mc = new MappingConfiguration(xpni.Current);
				if (!mc.IsEmpty())
				{
					Mappings.Add(mc);
				}
			}
		}

		private void ParseClassesCache(XPathNavigator navigator)
		{
			XPathNodeIterator xpni = navigator.Select(CfgXmlHelper.SessionFactoryClassesCacheExpression);
			while (xpni.MoveNext())
			{
				ClassesCache.Add(new ClassCacheConfiguration(xpni.Current));
			}
		}


		private void ParseCollectionsCache(XPathNavigator navigator)
		{
			XPathNodeIterator xpni = navigator.Select(CfgXmlHelper.SessionFactoryCollectionsCacheExpression);
			while (xpni.MoveNext())
			{
				CollectionsCache.Add(new CollectionCacheConfiguration(xpni.Current));
			}
		}


		private void ParseListeners(XPathNavigator navigator)
		{
			XPathNodeIterator xpni = navigator.Select(CfgXmlHelper.SessionFactoryListenersExpression);
			while (xpni.MoveNext())
			{
				Listeners.Add(new ListenerConfiguration(xpni.Current));
			}
		}

		private void ParseEvents(XPathNavigator navigator)
		{
			XPathNodeIterator xpni = navigator.Select(CfgXmlHelper.SessionFactoryEventsExpression);
			while (xpni.MoveNext())
			{
				Events.Add(new EventConfiguration(xpni.Current));
			}
		}
	}
}
