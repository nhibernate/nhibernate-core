using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;

namespace NHibernate.Cfg.ConfigurationSchema
{
	public class EventConfiguration
	{
		internal EventConfiguration(XPathNavigator eventElement)
		{
			Parse(eventElement);
		}

		public EventConfiguration(ListenerConfiguration listener, ListenerType type)
		{
			if (listener == null)
				throw new ArgumentNullException("listener");
			this.type = type;
			listeners.Add(listener);
		}

		private void Parse(XPathNavigator eventElement)
		{
			XPathNavigator eventClone = eventElement.Clone();
			eventElement.MoveToFirstAttribute();
			type = CfgXmlHelper.ListenerTypeConvertFrom(eventElement.Value);
			XPathNodeIterator listenersI = eventClone.SelectDescendants(XPathNodeType.Element, false);
			while (listenersI.MoveNext())
			{
				listeners.Add(new ListenerConfiguration(listenersI.Current));
			}
		}

		private ListenerType type;
		public ListenerType Type
		{
			get { return type; }
		}

		private IList<ListenerConfiguration> listeners = new List<ListenerConfiguration>();
		public IList<ListenerConfiguration> Listeners
		{
			get { return listeners; }
		}

	}
}
