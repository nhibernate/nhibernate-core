using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode.MappersTests.ClassMapperTests
{
	public class ComposedIdTests
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
		public void WhenClassWithComposedIdThenTheIdIsConpositeId()
		{
			var mapdoc = new HbmMapping();
			var mapper = new ClassMapper(typeof(Person), mapdoc, null);

			mapper.ComposedId(map =>
			{
				map.Property(For<Person>.Property(x => x.Email), pm => { });
				map.ManyToOne(For<Person>.Property(x => x.User), pm => { });
			});
			var hbmClass = mapdoc.RootClasses[0];
			hbmClass.Id.Should().Be.Null();
			var hbmCompositeId = hbmClass.CompositeId;
			hbmCompositeId.Should().Not.Be.Null();
			hbmCompositeId.@class.Should().Be.Null();
			hbmCompositeId.Items.Should().Have.Count.EqualTo(2);
			hbmCompositeId.Items.Select(x => x.GetType()).Should().Have.SameValuesAs(typeof(HbmKeyProperty), typeof(HbmKeyManyToOne));
		}

		[Test]
		public void WhenComposedIdCustomizedMoreThanOnceThenMerge()
		{
			var mapdoc = new HbmMapping();
			var mapper = new ClassMapper(typeof(Person), mapdoc, null);

			mapper.ComposedId(map => map.Property(For<Person>.Property(x => x.Email), pm => { }));
			mapper.ComposedId(map => map.ManyToOne(For<Person>.Property(x => x.User), pm => { }));

			var hbmClass = mapdoc.RootClasses[0];
			hbmClass.Id.Should().Be.Null();
			var hbmCompositeId = hbmClass.CompositeId;
			hbmCompositeId.Items.Should().Have.Count.EqualTo(2);
		}
	}
}