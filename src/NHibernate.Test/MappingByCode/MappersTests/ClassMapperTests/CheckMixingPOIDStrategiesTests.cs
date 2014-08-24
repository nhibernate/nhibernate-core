using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode.MappersTests.ClassMapperTests
{
	public class CheckMixingPoidStrategiesTests
	{
		private class PersonId
		{
			public string Email { get; set; }
		}

		private class Person
		{
			private PersonId id;
			public PersonId Id
			{
				get { return id; }
			}

			public int Poid { get; set; }
			public string Name { get; set; }
		}

		[Test]
		public void WhenMixComposedIdWithComponentAsIdThenThrows()
		{
			var mapdoc = new HbmMapping();
			var mapper = new ClassMapper(typeof (Person), mapdoc, For<Person>.Property(x => x.Id));

			mapper.ComposedId(map => map.Property(For<Person>.Property(x => x.Name), pm => { }));
			Executing.This(() =>
			               mapper.ComponentAsId(For<Person>.Property(x => x.Id), map => map.Property(For<PersonId>.Property(x => x.Email), pm => { }))
										 ).Should().Throw<MappingException>();
		}

		[Test]
		public void WhenMixComponentAsIdWithComposedIdThenThrows()
		{
			var mapdoc = new HbmMapping();
			var mapper = new ClassMapper(typeof(Person), mapdoc, For<Person>.Property(x => x.Id));

			mapper.ComponentAsId(For<Person>.Property(x => x.Id), map => map.Property(For<PersonId>.Property(x => x.Email), pm => { }));
			Executing.This(() =>
										 mapper.ComposedId(map => map.Property(For<Person>.Property(x => x.Name), pm => { }))
										 ).Should().Throw<MappingException>();
		}

		[Test]
		public void WhenMixComposedIdWithSimpleIdThenThrows()
		{
			var mapdoc = new HbmMapping();
			var mapper = new ClassMapper(typeof(Person), mapdoc, For<Person>.Property(x => x.Id));

			mapper.ComposedId(map => map.Property(For<Person>.Property(x => x.Name), pm => { }));
			Executing.This(() =>
										 mapper.Id(For<Person>.Property(x => x.Poid), pm => { })
										 ).Should().Throw<MappingException>();
		}

		[Test]
		public void WhenMixComponentAsIdWithSimpleIdThenThrows()
		{
			var mapdoc = new HbmMapping();
			var mapper = new ClassMapper(typeof(Person), mapdoc, For<Person>.Property(x => x.Id));

			mapper.ComponentAsId(For<Person>.Property(x => x.Id), map => map.Property(For<PersonId>.Property(x => x.Email), pm => { }));
			Executing.This(() =>
										 mapper.Id(For<Person>.Property(x => x.Poid), pm => { })
										 ).Should().Throw<MappingException>();
		}

		[Test]
		public void WhenMixSimpleIdWithComposedIdThenThrows()
		{
			var mapdoc = new HbmMapping();
			var mapper = new ClassMapper(typeof(Person), mapdoc, For<Person>.Property(x => x.Id));

			mapper.Id(For<Person>.Property(x => x.Poid), pm => { });
			Executing.This(() =>
										 mapper.ComposedId(map => map.Property(For<Person>.Property(x => x.Name), pm => { }))
										 ).Should().Throw<MappingException>();
		}

		[Test]
		public void WhenMixSimpleIdWithComponentAsIdThenThrows()
		{
			var mapdoc = new HbmMapping();
			var mapper = new ClassMapper(typeof(Person), mapdoc, For<Person>.Property(x => x.Id));

			mapper.Id(For<Person>.Property(x => x.Poid), pm => { });
			Executing.This(() =>
										 mapper.ComponentAsId(For<Person>.Property(x => x.Id), map => map.Property(For<PersonId>.Property(x => x.Email), pm => { }))
										 ).Should().Throw<MappingException>();
		}
	}
}