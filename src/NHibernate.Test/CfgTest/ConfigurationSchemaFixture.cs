using System;
using System.Configuration;
using NHibernate.Event;
using NUnit.Framework;
using NHibernate.Cfg;
using NHibernate.Cfg.ConfigurationSchema;
using System.Xml;

namespace NHibernate.Test.CfgTest
{
	[TestFixture]
	public class ConfigurationSchemaFixture
	{
		[Test]
		public void InvalidConfig()
		{
			string xml =
			@"<?xml version='1.0' encoding='utf-8' ?>
<hibernate-configuration xmlns='urn:nhibernate-configuration-2.2'>
		<bytecode-provider type='pizza'/>
</hibernate-configuration>";

			XmlTextReader xtr = new XmlTextReader(xml, XmlNodeType.Document, null);
			Assert.Throws<HibernateConfigException>(()=>new HibernateConfiguration(xtr));
		}

		[Test]
		public void FromAppConfigTest()
		{
			IHibernateConfiguration hc = ConfigurationManager.GetSection("hibernate-configuration") as IHibernateConfiguration;
			Assert.That(hc.ByteCodeProviderType, Is.EqualTo("lcg"));
			Assert.IsTrue(hc.UseReflectionOptimizer);
			Assert.AreEqual("NHibernate.Test", hc.SessionFactory.Name);
		}

		[Test]
		public void IgnoreSystemOutOfAppConfig()
		{
			IHibernateConfiguration hc = ConfigurationManager.GetSection("hibernate-configuration") as IHibernateConfiguration;
			string xml =
			@"<?xml version='1.0' encoding='utf-8' ?>
<hibernate-configuration xmlns='urn:nhibernate-configuration-2.2'>
		<bytecode-provider type='codedom'/>
		<reflection-optimizer use='false'/>
		<session-factory name='MyFactoryName'>
		</session-factory>
</hibernate-configuration>";

			XmlTextReader xtr = new XmlTextReader(xml, XmlNodeType.Document, null);
			HibernateConfiguration newhc = new HibernateConfiguration(xtr);
			Assert.AreEqual(hc.ByteCodeProviderType, newhc.ByteCodeProviderType);
			Assert.AreEqual(hc.UseReflectionOptimizer, newhc.UseReflectionOptimizer);
		}

		[Test]
		public void EmptyFactoryNotAllowed()
		{
			// session-factory omission not allowed out of App.config
			string xml =
			@"<?xml version='1.0' encoding='utf-8' ?>
<hibernate-configuration xmlns='urn:nhibernate-configuration-2.2'>
		<bytecode-provider type='codedom'/>
</hibernate-configuration>";

			XmlTextReader xtr = new XmlTextReader(xml, XmlNodeType.Document, null);
			Assert.Throws<HibernateConfigException>(()=> new HibernateConfiguration(xtr));
		}

		[Test]
		public void FactoryName()
		{
			string xml =
			@"<?xml version='1.0' encoding='utf-8' ?>
<hibernate-configuration xmlns='urn:nhibernate-configuration-2.2'>
	<session-factory name='MyFactoryName'>
	</session-factory>
</hibernate-configuration>";

			XmlTextReader xtr = new XmlTextReader(xml, XmlNodeType.Document, null);
			HibernateConfiguration hc = new HibernateConfiguration(xtr);
			Assert.AreEqual("MyFactoryName", hc.SessionFactory.Name);
		}

		[Test]
		public void Properties()
		{
				string xml =
				@"<?xml version='1.0' encoding='utf-8' ?>
<hibernate-configuration xmlns='urn:nhibernate-configuration-2.2'>
	<session-factory>
		<property name='connection.provider'>Value of connection.provider</property>
		<property name='default_schema'>Value of default_schema</property>
	</session-factory>
</hibernate-configuration>";

			XmlTextReader xtr = new XmlTextReader(xml, XmlNodeType.Document, null);
			HibernateConfiguration hc = new HibernateConfiguration(xtr);
			Assert.AreEqual(2, hc.SessionFactory.Properties.Count);
			Assert.IsTrue(hc.SessionFactory.Properties.ContainsKey("connection.provider"));
			Assert.IsTrue(hc.SessionFactory.Properties.ContainsKey("default_schema"));
			Assert.AreEqual("Value of connection.provider", hc.SessionFactory.Properties["connection.provider"]);
			Assert.AreEqual("Value of default_schema", hc.SessionFactory.Properties["default_schema"]);
		}

		[Test]
		public void Mappings()
		{
			string xml =
			@"<?xml version='1.0' encoding='utf-8' ?>
<hibernate-configuration xmlns='urn:nhibernate-configuration-2.2'>
	<session-factory>
		<mapping file='AFile'/>
		<mapping assembly='AAssembly'/>
		<mapping assembly='AAssemblyWithResource' resource='AResource'/>
		<mapping assembly='AAssemblyWithResource' resource='AnotherResource'/>
	</session-factory>
</hibernate-configuration>";

			XmlTextReader xtr = new XmlTextReader(xml, XmlNodeType.Document, null);
			HibernateConfiguration hc = new HibernateConfiguration(xtr);
			Assert.AreEqual(4, hc.SessionFactory.Mappings.Count);
			Assert.IsTrue(hc.SessionFactory.Mappings.Contains(new MappingConfiguration("AFile")));
			Assert.IsTrue(hc.SessionFactory.Mappings.Contains(new MappingConfiguration("AAssembly", null)));
			Assert.IsTrue(hc.SessionFactory.Mappings.Contains(new MappingConfiguration("AAssemblyWithResource", "AResource")));
			Assert.IsTrue(hc.SessionFactory.Mappings.Contains(new MappingConfiguration("AAssemblyWithResource", "AnotherResource")));
		}

		[Test]
		public void MappingEquatable()
		{
			// Whole assembly and assembly&resource are equals
			Assert.IsTrue((new MappingConfiguration("AAssembly", null)).Equals(
				(new MappingConfiguration("AAssembly", "SomeResource"))), "Whole assembly is not equal then partial assembly.");

			// Two different partial assembly are not equals
			Assert.IsFalse((new MappingConfiguration("AAssembly", "AnotherResource")).Equals(
				(new MappingConfiguration("AAssembly", "SomeResource"))));
			Assert.IsTrue((new MappingConfiguration("AAssembly", "")).Equals(
				(new MappingConfiguration("AAssembly", null))));
			Assert.IsTrue((new MappingConfiguration("AAssembly", "aResource")).Equals(
				(new MappingConfiguration("AAssembly", "aResource"))));

			Assert.IsTrue((new MappingConfiguration("AFile")).Equals(
				(new MappingConfiguration("AFile"))));
			Assert.IsFalse((new MappingConfiguration("AFile")).Equals(
				(new MappingConfiguration("AnotherFile"))));
			Assert.IsFalse((new MappingConfiguration("AFile")).Equals(
				(new MappingConfiguration("AAssembly", null))));
		}

		[Test]
		public void NotAllowedMappings()
		{
			string xml =
			@"<?xml version='1.0' encoding='utf-8' ?>
<hibernate-configuration xmlns='urn:nhibernate-configuration-2.2'>
	<session-factory>
		<mapping file='AFile' assembly='AAssembly'/>
	</session-factory>
</hibernate-configuration>";

			XmlTextReader xtr = new XmlTextReader(xml, XmlNodeType.Document, null);
			Assert.Throws<HibernateConfigException>(()=>new HibernateConfiguration(xtr));
		}

		[Test]
		public void ClassesCache()
		{
			string xml =
			@"<?xml version='1.0' encoding='utf-8' ?>
<hibernate-configuration xmlns='urn:nhibernate-configuration-2.2'>
	<session-factory>
		<class-cache class='Class1' usage='read-only' include='non-lazy' region='ARegion'/>
	</session-factory>
</hibernate-configuration>";

			XmlTextReader xtr = new XmlTextReader(xml, XmlNodeType.Document, null);
			HibernateConfiguration hc = new HibernateConfiguration(xtr);
			Assert.AreEqual(1, hc.SessionFactory.ClassesCache.Count);
			Assert.AreEqual("Class1", hc.SessionFactory.ClassesCache[0].Class);
			Assert.AreEqual(EntityCacheUsage.Readonly, hc.SessionFactory.ClassesCache[0].Usage);
			Assert.AreEqual(ClassCacheInclude.NonLazy, hc.SessionFactory.ClassesCache[0].Include);
			Assert.AreEqual("ARegion", hc.SessionFactory.ClassesCache[0].Region);
		}

		[Test]
		public void CollectionsCache()
		{
			string xml =
			@"<?xml version='1.0' encoding='utf-8' ?>
<hibernate-configuration xmlns='urn:nhibernate-configuration-2.2'>
	<session-factory>
		<collection-cache collection='Collection1' usage='nonstrict-read-write' region='ARegion'/>
	</session-factory>
</hibernate-configuration>";

			XmlTextReader xtr = new XmlTextReader(xml, XmlNodeType.Document, null);
			HibernateConfiguration hc = new HibernateConfiguration(xtr);
			Assert.AreEqual(1, hc.SessionFactory.CollectionsCache.Count);
			Assert.AreEqual("Collection1", hc.SessionFactory.CollectionsCache[0].Collection);
			Assert.AreEqual(EntityCacheUsage.NonStrictReadWrite, hc.SessionFactory.CollectionsCache[0].Usage);
			Assert.AreEqual("ARegion", hc.SessionFactory.CollectionsCache[0].Region);
		}

		[Test]
		public void Listeners()
		{
			string xml =
			@"<?xml version='1.0' encoding='utf-8' ?>
<hibernate-configuration xmlns='urn:nhibernate-configuration-2.2'>
	<session-factory>
		<listener class='AClass' type='auto-flush'/>
	</session-factory>
</hibernate-configuration>";

			XmlTextReader xtr = new XmlTextReader(xml, XmlNodeType.Document, null);
			HibernateConfiguration hc = new HibernateConfiguration(xtr);
			Assert.AreEqual(1, hc.SessionFactory.Listeners.Count);
			Assert.AreEqual("AClass", hc.SessionFactory.Listeners[0].Class);
			Assert.AreEqual(ListenerType.Autoflush, hc.SessionFactory.Listeners[0].Type);
		}

		[Test]
		public void Events()
		{
			string xml =
			@"<?xml version='1.0' encoding='utf-8' ?>
<hibernate-configuration xmlns='urn:nhibernate-configuration-2.2'>
	<session-factory>
		<event type='merge'>
			<listener class='AClass'/>
			<listener class='BClass'/>
		</event>
	</session-factory>
</hibernate-configuration>";

			XmlTextReader xtr = new XmlTextReader(xml, XmlNodeType.Document, null);
			HibernateConfiguration hc = new HibernateConfiguration(xtr);
			Assert.AreEqual(1, hc.SessionFactory.Events.Count);
			Assert.AreEqual(ListenerType.Merge, hc.SessionFactory.Events[0].Type);
			Assert.AreEqual(2, hc.SessionFactory.Events[0].Listeners.Count);
			Assert.AreEqual("AClass", hc.SessionFactory.Events[0].Listeners[0].Class);
		}
	}
}
