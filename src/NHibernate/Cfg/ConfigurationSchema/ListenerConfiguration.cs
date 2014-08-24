using System;
using System.Xml.XPath;
using NHibernate.Event;

namespace NHibernate.Cfg.ConfigurationSchema
{
	/// <summary>
	/// Configuration parsed values for a listener XML node
	/// </summary>
	public class ListenerConfiguration
	{
		internal ListenerConfiguration(XPathNavigator listenerElement)
		{
			Parse(listenerElement);
		}

		internal ListenerConfiguration(XPathNavigator listenerElement, ListenerType defaultType)
		{
			type = defaultType;
			Parse(listenerElement);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ListenerConfiguration"/> class.
		/// </summary>
		/// <param name="clazz">The class full name.</param>
		/// <exception cref="ArgumentException">When <paramref name="clazz"/> is null or empty.</exception>
		public ListenerConfiguration(string clazz)
		{
			if (string.IsNullOrEmpty(clazz))
				throw new ArgumentException("clazz is null or empty.", "clazz");
			this.clazz = clazz;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ListenerConfiguration"/> class.
		/// </summary>
		/// <param name="clazz">The class full name.</param>
		/// <param name="type">The listener type.</param>
		/// <exception cref="ArgumentException">When <paramref name="clazz"/> is null or empty.</exception>
		public ListenerConfiguration(string clazz, ListenerType type)
			: this(clazz)
		{
			this.type = type;
		}

		private void Parse(XPathNavigator listenerElement)
		{
			if (listenerElement.MoveToFirstAttribute())
			{
				do
				{
					switch (listenerElement.Name)
					{
						case "class":
							if (listenerElement.Value.Trim().Length == 0)
								throw new HibernateConfigException("Invalid listener element; the attribute <class> must be assigned with no empty value");
							clazz = listenerElement.Value;
							break;
						case "type":
							type = CfgXmlHelper.ListenerTypeConvertFrom(listenerElement.Value);
							break;
					}
				}
				while (listenerElement.MoveToNextAttribute());
			}
		}

		private string clazz;
		/// <summary>
		/// The class full name.
		/// </summary>
		public string Class
		{
			get { return clazz; }
		}

		private ListenerType type= ListenerType.NotValidType;
		/// <summary>
		/// The listener type.
		/// </summary>
		/// <remarks>Default value <see cref="ListenerType.NotValidType"/> mean that the value is ignored.</remarks>
		public ListenerType Type
		{
			get { return type; }
		}

	}
}
