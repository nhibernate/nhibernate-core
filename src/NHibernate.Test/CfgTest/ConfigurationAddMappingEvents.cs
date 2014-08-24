using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg;
using NHibernate.Cfg.Loquacious;
using NHibernate.Dialect;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.CfgTest
{
	public class ConfigurationAddMappingEvents
	{
		private const string ProductLineMapping =
			@"<?xml version='1.0' encoding='utf-8' ?>
<hibernate-mapping xmlns='urn:nhibernate-mapping-2.2'>
	<class entity-name='ProductLine'>
		<id name='Id' type='int'>
			<generator class='hilo'/>
		</id>
		<property name='Description' not-null='true' length='200' type='string'/>
		<bag name='Models' cascade='all' inverse='true'>
			<key column='productId'/>
			<one-to-many class='Model'/>
		</bag>
	</class>
</hibernate-mapping>
";
		private const string ModelMapping =
			@"<?xml version='1.0' encoding='utf-8' ?>
<hibernate-mapping xmlns='urn:nhibernate-mapping-2.2'>
	<class entity-name='Model'>
		<id name='Id' type='int'>
			<generator class='hilo'/>
		</id>

		<property name='Name' not-null='true' length='25' type='string'/>
		<property name='Description' not-null='true' length='200' type='string'/>
		<many-to-one name='ProductLine' column='productId' not-null='true' class='ProductLine'/>
	</class>
</hibernate-mapping>
";
		[Test]
		public void WhenSubscribedToBeforeBindThenRaiseEventForEachMapping()
		{
			var listOfCalls = new List<BindMappingEventArgs>();
			var configuration = new Configuration();
			configuration.DataBaseIntegration(x => x.Dialect<MsSql2008Dialect>());
			configuration.BeforeBindMapping += (sender, args) => { sender.Should().Be.SameInstanceAs(configuration); listOfCalls.Add(args); };

			configuration.AddXmlString(ProductLineMapping);
			configuration.AddXmlString(ModelMapping);

			listOfCalls.Count.Should().Be(2);
			listOfCalls.Select(x => x.FileName).All(x => x.Satisfy(filename => filename != null));
			listOfCalls.Select(x => x.Mapping).All(x => x.Satisfy(mappingDoc => mappingDoc != null));
			listOfCalls.Select(x => x.Dialect).All(x => x.Satisfy(dialect => dialect.GetType() == typeof(MsSql2008Dialect)));
		}

		[Test]
		public void WhenSubscribedToAfterBindThenRaiseEventForEachMapping()
		{
			var listOfCalls = new List<BindMappingEventArgs>();
			var configuration = new Configuration();
			configuration.DataBaseIntegration(x => x.Dialect<MsSql2008Dialect>());
			configuration.AfterBindMapping += (sender, args) => { sender.Should().Be.SameInstanceAs(configuration); listOfCalls.Add(args); };

			configuration.AddXmlString(ProductLineMapping);
			configuration.AddXmlString(ModelMapping);

			listOfCalls.Count.Should().Be(2);
			listOfCalls.Select(x => x.FileName).All(x => x.Satisfy(filename => filename != null));
			listOfCalls.Select(x => x.Mapping).All(x => x.Satisfy(mappingDoc => mappingDoc != null));
			listOfCalls.Select(x => x.Dialect).All(x => x.Satisfy(dialect => dialect.GetType() == typeof(MsSql2008Dialect)));
		}
	}
}