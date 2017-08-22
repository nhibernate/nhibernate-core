using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;

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
			// Assigned by reflection
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
			private PersonId id;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value
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
			Assert.That(() => mapper.ComponentAsId(For<Person>.Property(x => x.Id), map => map.Property(For<PersonId>.Property(x => x.Email), pm => { })), Throws.TypeOf<MappingException>());
		}

		[Test]
		public void WhenMixComponentAsIdWithComposedIdThenThrows()
		{
			var mapdoc = new HbmMapping();
			var mapper = new ClassMapper(typeof(Person), mapdoc, For<Person>.Property(x => x.Id));

			mapper.ComponentAsId(For<Person>.Property(x => x.Id), map => map.Property(For<PersonId>.Property(x => x.Email), pm => { }));
			Assert.That(() => mapper.ComposedId(map => map.Property(For<Person>.Property(x => x.Name), pm => { })), Throws.TypeOf<MappingException>());
		}

		[Test]
		public void WhenMixComposedIdWithSimpleIdThenThrows()
		{
			var mapdoc = new HbmMapping();
			var mapper = new ClassMapper(typeof(Person), mapdoc, For<Person>.Property(x => x.Id));

			mapper.ComposedId(map => map.Property(For<Person>.Property(x => x.Name), pm => { }));
			Assert.That(() => mapper.Id(For<Person>.Property(x => x.Poid), pm => { }), Throws.TypeOf<MappingException>());
		}

		[Test]
		public void WhenMixComponentAsIdWithSimpleIdThenThrows()
		{
			var mapdoc = new HbmMapping();
			var mapper = new ClassMapper(typeof(Person), mapdoc, For<Person>.Property(x => x.Id));

			mapper.ComponentAsId(For<Person>.Property(x => x.Id), map => map.Property(For<PersonId>.Property(x => x.Email), pm => { }));
			Assert.That(() => mapper.Id(For<Person>.Property(x => x.Poid), pm => { }), Throws.TypeOf<MappingException>());
		}

		[Test]
		public void WhenMixSimpleIdWithComposedIdThenThrows()
		{
			var mapdoc = new HbmMapping();
			var mapper = new ClassMapper(typeof(Person), mapdoc, For<Person>.Property(x => x.Id));

			mapper.Id(For<Person>.Property(x => x.Poid), pm => { });
			Assert.That(() => mapper.ComposedId(map => map.Property(For<Person>.Property(x => x.Name), pm => { })), Throws.TypeOf<MappingException>());
		}

		[Test]
		public void WhenMixSimpleIdWithComponentAsIdThenThrows()
		{
			var mapdoc = new HbmMapping();
			var mapper = new ClassMapper(typeof(Person), mapdoc, For<Person>.Property(x => x.Id));

			mapper.Id(For<Person>.Property(x => x.Poid), pm => { });
			Assert.That(() => mapper.ComponentAsId(For<Person>.Property(x => x.Id), map => map.Property(For<PersonId>.Property(x => x.Email), pm => { })), Throws.TypeOf<MappingException>());
		}
	}
}