using System;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode.ExpliticMappingTests
{
	public class ClassWithComponentsTest
	{
		public class Person1
		{
			public int Id { get; set; }
			public string Test { get; set; }
			public Name Name { get; set; }
			public Address Address { get; set; }
		}
		public class Name
		{
			public string First { get; set; }
			public string Last { get; set; }
		}

		public class Address
		{
			public string Street { get; set; }
			public int CivicNumber { get; set; }
		}
		[Test]
		public void ComponentMappingJustOnceDemo()
		{
			var mapper = new ModelMapper();
			mapper.Component<Name>(comp =>
			{
				comp.Property(name => name.First);
				comp.Property(name => name.Last);
			});
			mapper.Component<Address>(comp =>
			{
				comp.Property(address => address.CivicNumber);
				comp.Property(address => address.Street);
			});
			mapper.Class<Person1>(cm =>
			{
				cm.Id(person => person.Id, map => map.Generator(Generators.HighLow));
				cm.Property(person => person.Test);
				cm.Component(person => person.Name, comp => { });
				cm.Component(person => person.Address, comp => { });
			});

			var hbmMapping = mapper.CompileMappingForAllExplicitlyAddedEntities();

			var hbmClass = hbmMapping.RootClasses[0];

			var hbmComponents = hbmClass.Properties.OfType<HbmComponent>();
			hbmComponents.Should().Have.Count.EqualTo(2);
			hbmComponents.Select(x => x.Name).Should().Have.SameValuesAs("Name","Address");
		} 
	}
}