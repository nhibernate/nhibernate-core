using System;
using System.Xml;
using System.Xml.XPath;


namespace NHibernate.Cfg.ConfigurationSchema
{
	/// <summary>
	/// Values for bytecode-provider system property.
	/// </summary>
	public enum BytecodeProviderType
	{
		/// <summary>Xml value: lcg</summary>
		Lcg,
		/// <summary>Xml value: null</summary>
		Null
	}

	/// <summary>
	/// Configuration parsed values for hibernate-configuration section.
	/// </summary>
	public class HibernateConfiguration : IHibernateConfiguration
	{
		private static readonly INHibernateLogger log = NHibernateLogger.For(typeof(HibernateConfiguration));

		/// <summary>
		/// Initializes a new instance of the <see cref="HibernateConfiguration"/> class.
		/// </summary>
		/// <param name="hbConfigurationReader">The XML reader to parse.</param>
		/// <remarks>
		/// The nhibernate-configuration.xsd is applied to the XML.
		/// </remarks>
		/// <exception cref="HibernateConfigException">When nhibernate-configuration.xsd can't be applied.</exception>
		public HibernateConfiguration(XmlReader hbConfigurationReader)
			: this(hbConfigurationReader, false)
		{
		}

		private HibernateConfiguration(XmlReader hbConfigurationReader, bool fromAppSetting)
		{
			XPathNavigator nav;
			try
			{
				nav = new XPathDocument(XmlReader.Create(hbConfigurationReader, GetSettings())).CreateNavigator();
			}
			catch (HibernateConfigException)
			{
				throw;
			}
			catch (Exception e)
			{
				// Encapsulate and reThrow
				throw new HibernateConfigException(e);
			}
			Parse(nav, fromAppSetting);
		}

		public static HibernateConfiguration FromAppConfig(XmlNode node)
		{
			return FromAppConfig(node.OuterXml);
		}

		public static HibernateConfiguration FromAppConfig(string xml)
		{
			using (var reader = new XmlTextReader(xml, XmlNodeType.Document, null))
			{
				return new HibernateConfiguration(reader, true);
			}
		}

		private XmlReaderSettings GetSettings()
		{
			XmlReaderSettings xmlrs = (new XmlSchemas()).CreateConfigReaderSettings();
			return xmlrs;
		}

		private void Parse(XPathNavigator navigator, bool fromAppConfig)
		{
			ParseByteCodeProvider(navigator, fromAppConfig);
			ParseObjectsFactory(navigator, fromAppConfig);
			ParseReflectionOptimizer(navigator, fromAppConfig);
			XPathNavigator xpn = navigator.SelectSingleNode(CfgXmlHelper.SessionFactoryExpression);
			if (xpn != null)
			{
				sessionFactory = new SessionFactoryConfiguration(navigator);
			}
			else
			{
				if (!fromAppConfig)
				{
					throw new HibernateConfigException("<session-factory xmlns='" + CfgXmlHelper.CfgSchemaXMLNS +
					"'> element was not found in the configuration file.");
				}
			}
		}

		private void ParseByteCodeProvider(XPathNavigator navigator, bool fromAppConfig)
		{
			XPathNavigator xpn = navigator.SelectSingleNode(CfgXmlHelper.ByteCodeProviderExpression);
			if (xpn != null)
			{
				if (fromAppConfig)
				{
					xpn.MoveToFirstAttribute();
					byteCodeProviderType = xpn.Value;
				}
				else
				{
					LogWarnIgnoredProperty("bytecode-provider");
				}
			}
		}

		private void ParseObjectsFactory(XPathNavigator navigator, bool fromAppConfig)
		{
			var xpn = navigator.SelectSingleNode(CfgXmlHelper.ObjectsFactoryExpression);
			if (xpn != null)
			{
				if (fromAppConfig)
				{
					xpn.MoveToFirstAttribute();
					ObjectsFactoryType = xpn.Value;
				}
				else
				{
					LogWarnIgnoredProperty("objects-factory");
				}
			}
		}

		private static void LogWarnIgnoredProperty(string propName)
		{
			if (log.IsWarnEnabled())
				log.Warn("{0} property is ignored out of application configuration file.", propName);
		}

		private void ParseReflectionOptimizer(XPathNavigator navigator, bool fromAppConfig)
		{
			XPathNavigator xpn = navigator.SelectSingleNode(CfgXmlHelper.ReflectionOptimizerExpression);
			if (xpn != null)
			{
				if (fromAppConfig)
				{
					xpn.MoveToFirstAttribute();
					useReflectionOptimizer = xpn.ValueAsBoolean;
				}
				else
				{
					LogWarnIgnoredProperty("reflection-optimizer");
				}
			}
		}

		private string byteCodeProviderType = BytecodeProviderType.Lcg.ToConfigurationString();
		/// <summary>
		/// Value for bytecode-provider system property.
		/// </summary>
		/// <remarks>Default value <see cref="BytecodeProviderType.Lcg"/>.</remarks>
		public string ByteCodeProviderType
		{
			get { return byteCodeProviderType; }
		}

		/// <summary>
		/// Value for objects-factory system property.
		/// </summary>
		/// <remarks>Default value <see langword="null" />.</remarks>
		// 6.0 TODO add to IHibernateConfiguration
		public string ObjectsFactoryType { get; private set; }

		private bool useReflectionOptimizer = true;
		/// <summary>
		/// Value for reflection-optimizer system property.
		/// </summary>
		/// <remarks>Default value true.</remarks>
		public bool UseReflectionOptimizer
		{
			get { return useReflectionOptimizer; }
		}

		private SessionFactoryConfiguration sessionFactory;
		/// <summary>
		/// The <see cref="SessionFactoryConfiguration"/> if the session-factory exists in hibernate-configuration;
		/// Otherwise null.
		/// </summary>
		public ISessionFactoryConfiguration SessionFactory
		{
			get { return sessionFactory; }
		}
	}
}
