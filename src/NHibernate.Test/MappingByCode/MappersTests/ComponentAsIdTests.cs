using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;
using SharpTestsEx;

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

			component.@class.Should().Contain("PersonId");
		}

		[Test]
		public void CanMapProperty()
		{
			var mapdoc = new HbmMapping();
			var compositeId = new HbmCompositeId();
			var mapper = new ComponentAsIdMapper(typeof(PersonId), For<Person>.Property(x => x.Id), compositeId, mapdoc);

			mapper.Property(For<PersonId>.Property(ts => ts.Email), x => { });

			compositeId.Items.Should().Have.Count.EqualTo(1);
			compositeId.Items.First().Should().Be.OfType<HbmKeyProperty>();
			compositeId.Items.OfType<HbmKeyProperty>().First().Name.Should().Be.EqualTo("Email");
		}

		[Test]
		public void CallPropertyMapper()
		{
			var mapdoc = new HbmMapping();
			var compositeId = new HbmCompositeId();
			var mapper = new ComponentAsIdMapper(typeof(PersonId), For<Person>.Property(x => x.Id), compositeId, mapdoc);
			var called = false;

			mapper.Property(For<PersonId>.Property(ts => ts.Email), x => called = true);

			called.Should().Be.True();
		}

		[Test]
		public void CanMapManyToOne()
		{
			var mapdoc = new HbmMapping();
			var compositeId = new HbmCompositeId();
			var mapper = new ComponentAsIdMapper(typeof(PersonId), For<Person>.Property(x => x.Id), compositeId, mapdoc);

			mapper.ManyToOne(For<PersonId>.Property(ts => ts.User), x => { });

			compositeId.Items.Should().Have.Count.EqualTo(1);
			compositeId.Items.First().Should().Be.OfType<HbmKeyManyToOne>();
			compositeId.Items.OfType<HbmKeyManyToOne>().First().Name.Should().Be.EqualTo("User");
		}

		[Test]
		public void CallMapManyToOneMapper()
		{
			var mapdoc = new HbmMapping();
			var compositeId = new HbmCompositeId();
			var mapper = new ComponentAsIdMapper(typeof(PersonId), For<Person>.Property(x => x.Id), compositeId, mapdoc);
			var called = false;

			mapper.ManyToOne(For<PersonId>.Property(ts => ts.User), x => called = true);

			called.Should().Be.True();
		}
	}
}