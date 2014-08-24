using System;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;
using SharpTestsEx;

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
			hbmClass.Id.Should().Be.Null();
			var hbmCompositeId = hbmClass.CompositeId;
			hbmCompositeId.Should().Not.Be.Null();
			hbmCompositeId.@class.Should().Not.Be.Null();
			hbmCompositeId.Items.Should().Have.Count.EqualTo(2);
			hbmCompositeId.Items.Select(x => x.GetType()).Should().Have.SameValuesAs(typeof(HbmKeyProperty),typeof(HbmKeyManyToOne));
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
			hbmClass.Id.Should().Be.Null();
			var hbmCompositeId = hbmClass.CompositeId;
			hbmCompositeId.Items.Should().Have.Count.EqualTo(2);
			hbmCompositeId.access.Should().Contain("field");
		}

		[Test]
		public void WhenMapExternalMemberAsComponentIdThenThrows()
		{
			var mapdoc = new HbmMapping();
			var mapper = new ClassMapper(typeof(Person), mapdoc, For<Person>.Property(x => x.Id));

			mapper.Executing(m => m.ComponentAsId(For<User>.Property(x => x.Id), map => map.Access(Accessor.Field))).Throws<ArgumentOutOfRangeException>();
		}

	}
}