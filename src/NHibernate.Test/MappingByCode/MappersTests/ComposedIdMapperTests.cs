using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode.MappersTests
{
	public class ComposedIdMapperTests
	{
		private class Person
		{
			public string Email { get; set; }
			public User User { get; set; }
		}

		private class User
		{
			public int Id { get; set; }
		}

		[Test]
		public void CanMapProperty()
		{
			var mapdoc = new HbmMapping();
			var compositeId = new HbmCompositeId();
			var mapper = new ComposedIdMapper(typeof(Person), compositeId, mapdoc);

			mapper.Property(For<Person>.Property(ts => ts.Email), x => { });

			compositeId.Items.Should().Have.Count.EqualTo(1);
			compositeId.Items.First().Should().Be.OfType<HbmKeyProperty>();
			compositeId.Items.OfType<HbmKeyProperty>().First().Name.Should().Be.EqualTo("Email");
		}

		[Test]
		public void CallPropertyMapper()
		{
			var mapdoc = new HbmMapping();
			var compositeId = new HbmCompositeId();
			var mapper = new ComposedIdMapper(typeof(Person), compositeId, mapdoc);
			var called = false;

			mapper.Property(For<Person>.Property(ts => ts.Email), x => called = true);

			called.Should().Be.True();
		}

		[Test]
		public void CanMapManyToOne()
		{
			var mapdoc = new HbmMapping();
			var compositeId = new HbmCompositeId();
			var mapper = new ComposedIdMapper(typeof(Person), compositeId, mapdoc);

			mapper.ManyToOne(For<Person>.Property(ts => ts.User), x => { });

			compositeId.Items.Should().Have.Count.EqualTo(1);
			compositeId.Items.First().Should().Be.OfType<HbmKeyManyToOne>();
			compositeId.Items.OfType<HbmKeyManyToOne>().First().Name.Should().Be.EqualTo("User");
		}

		[Test]
		public void CallMapManyToOneMapper()
		{
			var mapdoc = new HbmMapping();
			var compositeId = new HbmCompositeId();
			var mapper = new ComposedIdMapper(typeof(Person), compositeId, mapdoc);
			var called = false;

			mapper.ManyToOne(For<Person>.Property(ts => ts.User), x => called = true);

			called.Should().Be.True();
		}
	}
}