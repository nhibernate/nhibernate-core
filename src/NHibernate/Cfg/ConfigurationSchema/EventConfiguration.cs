using System;
using System.Collections.Generic;
using System.Xml.XPath;
using NHibernate.Event;

namespace NHibernate.Cfg.ConfigurationSchema
{
	/// <summary>
	/// Configuration parsed values for a event XML node.
	/// </summary>
	public class EventConfiguration
	{
		internal EventConfiguration(XPathNavigator eventElement)
		{
			Parse(eventElement);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EventConfiguration"/> class.
		/// </summary>
		/// <param name="listener">The listener.</param>
		/// <param name="type">The type.</param>
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
				listeners.Add(new ListenerConfiguration(listenersI.Current, type));
			}
		}

		private ListenerType type;
		/// <summary>
		/// The default type of listeners.
		/// </summary>
		public ListenerType Type
		{
			get { return type; }
		}

		private IList<ListenerConfiguration> listeners = new List<ListenerConfiguration>();
		/// <summary>
		/// Listeners for this event.
		/// </summary>
		public IList<ListenerConfiguration> Listeners
		{
			get { return listeners; }
		}

	}
}
