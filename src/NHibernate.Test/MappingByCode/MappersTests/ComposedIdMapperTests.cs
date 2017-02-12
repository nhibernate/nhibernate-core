using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;

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

			Assert.That(compositeId.Items, Has.Length.EqualTo(1));
			Assert.That(compositeId.Items.First(), Is.TypeOf<HbmKeyProperty>());
			Assert.That(compositeId.Items.OfType<HbmKeyProperty>().First().Name, Is.EqualTo("Email"));
		}

		[Test]
		public void CallPropertyMapper()
		{
			var mapdoc = new HbmMapping();
			var compositeId = new HbmCompositeId();
			var mapper = new ComposedIdMapper(typeof(Person), compositeId, mapdoc);
			var called = false;

			mapper.Property(For<Person>.Property(ts => ts.Email), x => called = true);

			Assert.That(called, Is.True);
		}

		[Test]
		public void CanMapManyToOne()
		{
			var mapdoc = new HbmMapping();
			var compositeId = new HbmCompositeId();
			var mapper = new ComposedIdMapper(typeof(Person), compositeId, mapdoc);

			mapper.ManyToOne(For<Person>.Property(ts => ts.User), x => { });

			Assert.That(compositeId.Items, Has.Length.EqualTo(1));
			Assert.That(compositeId.Items.First(), Is.TypeOf<HbmKeyManyToOne>());
			Assert.That(compositeId.Items.OfType<HbmKeyManyToOne>().First().Name, Is.EqualTo("User"));
		}

		[Test]
		public void CallMapManyToOneMapper()
		{
			var mapdoc = new HbmMapping();
			var compositeId = new HbmCompositeId();
			var mapper = new ComposedIdMapper(typeof(Person), compositeId, mapdoc);
			var called = false;

			mapper.ManyToOne(For<Person>.Property(ts => ts.User), x => called = true);

			Assert.That(called, Is.True);
		}
	}
}