using System;
using System.Collections;
using System.Xml;
using System.IO;

using NUnit.Framework;

using NHibernate.Cfg;
using NHibernate.Mapping;

namespace NHibernate.Test.CfgTest
{
	[TestFixture]
	public class BinderFixture
	{
		private XmlDocument LoadAndValidate(string xml)
		{
			using( StringReader stringReader = new StringReader( xml ) )
			{
				XmlTextReader xmlReader = new XmlTextReader( stringReader );
				Configuration cfg = new Configuration();
				return cfg.LoadMappingDocument( xmlReader );
			}
		}

		[Test]
		public void DefaultVersionUnsavedValueIsUndefined()
		{
			string XML = @"<?xml version='1.0' ?>
<hibernate-mapping xmlns='urn:nhibernate-mapping-2.0'>
	<class name='class'>
		<id column='id'>
			<generator class='generator' />
		</id>
		<version name='version' />
	</class>
</hibernate-mapping>";

			XmlDocument document = LoadAndValidate( XML );
 			XmlNamespaceManager nsmgr = new XmlNamespaceManager( document.NameTable );
			nsmgr.AddNamespace( "hbm", "urn:nhibernate-mapping-2.0" );

			XmlNodeList list = document.SelectNodes( "//hbm:version", nsmgr );
			XmlNode node = list[0];
			SimpleValue model = new SimpleValue();
			Binder.MakeVersion(node, model);
			Assert.AreEqual("undefined", model.NullValue);
		}

		[Test]
		public void DefaultTimestampUnsavedValueIsUndefined()
		{
			string XML = @"<?xml version='1.0' ?>
<hibernate-mapping xmlns='urn:nhibernate-mapping-2.0'>
	<class name='class'>
		<id column='id'>
			<generator class='generator' />
		</id>
		<timestamp name='timestamp' />
	</class>
</hibernate-mapping>";

			XmlDocument document = LoadAndValidate( XML );
			XmlNamespaceManager nsmgr = new XmlNamespaceManager( document.NameTable );
			nsmgr.AddNamespace( "hbm", "urn:nhibernate-mapping-2.0" );

			XmlNodeList list = document.SelectNodes( "//hbm:timestamp", nsmgr );
			XmlNode node = list[0];
			SimpleValue model = new SimpleValue();
			Binder.MakeVersion(node, model);
			Assert.AreEqual("undefined", model.NullValue);
		}
	}
}
