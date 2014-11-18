using System;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.MappersTests.ClassMapperTests
{
	public class ComponetAsIdTests
	{
		private class PersonId
		{
			public string Email { get; set; }
			public User User { get; set; }
		}

		private class Person
		{
			private PersonId id;
			public PersonId Id
			{
				get { return id; }
			}

			public string Name { get; set; }
		}

		private class User
		{
			public int Id { get; set; }
		}

		[Test]
		public void WhenClassWithComponentIdThenTheIdIsConpositeId()
		{
			var mapdoc = new HbmMapping();
			var mapper = new ClassMapper(typeof(Person), mapdoc, For<Person>.Property(x => x.Id));

			mapper.ComponentAsId(For<Person>.Property(x => x.Id), map =>
																  {
																															map.Property(For<PersonId>.Property(x => x.Email), pm => { });
																															map.ManyToOne(For<PersonId>.Property(x => x.User), pm => { });
																														});
			var hbmClass = mapdoc.RootClasses[0];
			Assert.That(hbmClass.Id, Is.Null);
			var hbmCompositeId = hbmClass.CompositeId;
			Assert.That(hbmCompositeId, Is.Not.Null);
			Assert.That(hbmCompositeId.@class, Is.Not.Null);
			Assert.That(hbmCompositeId.Items, Has.Length.EqualTo(2));
			Assert.That(hbmCompositeId.Items.Select(x => x.GetType()), Is.EquivalentTo(new [] {typeof(HbmKeyProperty), typeof(HbmKeyManyToOne)}));
		}

		[Test]
		public void WhenComponentIdCustomizedMoreThanOnceThenMerge()
		{
			var mapdoc = new HbmMapping();
			var mapper = new ClassMapper(typeof(Person), mapdoc, For<Person>.Property(x => x.Id));

			mapper.ComponentAsId(For<Person>.Property(x => x.Id), map =>
			{
				map.Property(For<PersonId>.Property(x => x.Email), pm => { });
				map.ManyToOne(For<PersonId>.Property(x => x.User), pm => { });
			});
			mapper.ComponentAsId(For<Person>.Property(x => x.Id), map => map.Access(Accessor.Field));

			var hbmClass = mapdoc.RootClasses[0];
			Assert.That(hbmClass.Id, Is.Null);
			var hbmCompositeId = hbmClass.CompositeId;
			Assert.That(hbmCompositeId.Items, Has.Length.EqualTo(2));
			Assert.That(hbmCompositeId.access, Is.StringContaining("field"));
		}

		[Test]
		public void WhenMapExternalMemberAsComponentIdThenThrows()
		{
			var mapdoc = new HbmMapping();
			var mapper = new ClassMapper(typeof(Person), mapdoc, For<Person>.Property(x => x.Id));

			Assert.That(() => mapper.ComponentAsId(For<User>.Property(x => x.Id), map => map.Access(Accessor.Field)), Throws.TypeOf<ArgumentOutOfRangeException>());
		}

	}
}