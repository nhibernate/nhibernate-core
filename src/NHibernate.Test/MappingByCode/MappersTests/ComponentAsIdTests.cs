using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.MappersTests
{
	[TestFixture]
	public class ComponentAsIdTests
	{
		private class PersonId
		{
			public string Email { get; set; }
			public User User { get; set; }
		}

		private class Person
		{
			public PersonId Id { get; set; }
			public string Name { get; set; }
		}

		private class User
		{
			public int Id { get; set; }
		}

		[Test]
		public void WhenCreatedThenSetTheComponentClass()
		{
			var mapdoc = new HbmMapping();
			var component = new HbmCompositeId();
			new ComponentAsIdMapper(typeof(PersonId), For<Person>.Property(x=> x.Id), component, mapdoc);

			Assert.That(component.@class, Is.StringContaining("PersonId"));
		}

		[Test]
		public void CanMapProperty()
		{
			var mapdoc = new HbmMapping();
			var compositeId = new HbmCompositeId();
			var mapper = new ComponentAsIdMapper(typeof(PersonId), For<Person>.Property(x => x.Id), compositeId, mapdoc);

			mapper.Property(For<PersonId>.Property(ts => ts.Email), x => { });

			Assert.That(compositeId.Items, Has.Length.EqualTo(1));
			Assert.That(compositeId.Items.First(), Is.TypeOf<HbmKeyProperty>());
			Assert.That(compositeId.Items.OfType<HbmKeyProperty>().First().Name, Is.EqualTo("Email"));
		}

		[Test]
		public void CallPropertyMapper()
		{
			var mapdoc = new HbmMapping();
			var compositeId = new HbmCompositeId();
			var mapper = new ComponentAsIdMapper(typeof(PersonId), For<Person>.Property(x => x.Id), compositeId, mapdoc);
			var called = false;

			mapper.Property(For<PersonId>.Property(ts => ts.Email), x => called = true);

			Assert.That(called, Is.True);
		}

		[Test]
		public void CanMapManyToOne()
		{
			var mapdoc = new HbmMapping();
			var compositeId = new HbmCompositeId();
			var mapper = new ComponentAsIdMapper(typeof(PersonId), For<Person>.Property(x => x.Id), compositeId, mapdoc);

			mapper.ManyToOne(For<PersonId>.Property(ts => ts.User), x => { });

			Assert.That(compositeId.Items, Has.Length.EqualTo(1));
			Assert.That(compositeId.Items.First(), Is.TypeOf<HbmKeyManyToOne>());
			Assert.That(compositeId.Items.OfType<HbmKeyManyToOne>().First().Name, Is.EqualTo("User"));
		}

		[Test]
		public void CallMapManyToOneMapper()
		{
			var mapdoc = new HbmMapping();
			var compositeId = new HbmCompositeId();
			var mapper = new ComponentAsIdMapper(typeof(PersonId), For<Person>.Property(x => x.Id), compositeId, mapdoc);
			var called = false;

			mapper.ManyToOne(For<PersonId>.Property(ts => ts.User), x => called = true);

			Assert.That(called, Is.True);
		}
	}
}