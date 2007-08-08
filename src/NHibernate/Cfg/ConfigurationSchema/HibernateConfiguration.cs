using System;
using System.Xml;
using System.Xml.XPath;

namespace NHibernate.Cfg.ConfigurationSchema
{
	public enum BytecodeProviderType
	{
		Codedom,
		Lcg,
		Null
	}

	public class HibernateConfiguration : IHibernateConfiguration
	{
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
				// Encapsule and reThrow
				throw new HibernateConfigException(e);
			}
			Parse(nav, fromAppSetting);
		}

		internal static HibernateConfiguration FromAppConfig(XmlNode node)
		{
			XmlTextReader reader = new XmlTextReader(node.OuterXml, XmlNodeType.Document, null);
			return new HibernateConfiguration(reader, true);
		}

		private XmlReaderSettings GetSettings()
		{
			XmlReaderSettings xmlrs = (new XmlSchemas()).CreateConfigReaderSettings();
			return xmlrs;
		}

		private void Parse(XPathNavigator navigator, bool fromAppConfig)
		{
			ParseByteCodeProvider(navigator, fromAppConfig);
			ParseReflectionOptimizer(navigator, fromAppConfig);
			XPathNavigator xpn = navigator.SelectSingleNode(CfgXmlHelper.SessionFactoryExpression);
			if (xpn != null)
			{
				sessionFactory = new SessionFactoryConfiguration(navigator);
			}
			else
			{
				if (!fromAppConfig)
					throw new HibernateConfigException("<session-factory xmlns='" + CfgXmlHelper.CfgSchemaXMLNS +
					"'> element was not found in the configuration file.");
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
					byteCodeProviderType = CfgXmlHelper.ByteCodeProviderConvertFrom(xpn.Value);
				}
				else
				{
					// TODO: Warning for ByteCodeProvider ignored
				}
			}
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
					// TODO: Warning for ReflectionOptimizer ignored
				}
			}
		}

		private BytecodeProviderType byteCodeProviderType = BytecodeProviderType.Lcg;
		public BytecodeProviderType ByteCodeProviderType
		{
			get { return byteCodeProviderType; }
		}

		private bool useReflectionOptimizer = true;
		public bool UseReflectionOptimizer
		{
			get { return useReflectionOptimizer; }
		}

		private SessionFactoryConfiguration sessionFactory;
		public SessionFactoryConfiguration SessionFactory
		{
			get { return sessionFactory; }
		}
	}
}
