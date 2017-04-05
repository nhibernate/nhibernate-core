using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;

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
			Assert.That(hbmClass.Id, Is.Null);
			var hbmCompositeId = hbmClass.CompositeId;
			Assert.That(hbmCompositeId, Is.Not.Null);
			Assert.That(hbmCompositeId.@class, Is.Null);
			Assert.That(hbmCompositeId.Items, Has.Length.EqualTo(2));
			Assert.That(hbmCompositeId.Items.Select(x => x.GetType()), Is.EquivalentTo(new [] {typeof(HbmKeyProperty), typeof(HbmKeyManyToOne)}));
		}

		[Test]
		public void WhenComposedIdCustomizedMoreThanOnceThenMerge()
		{
			var mapdoc = new HbmMapping();
			var mapper = new ClassMapper(typeof(Person), mapdoc, null);

			mapper.ComposedId(map => map.Property(For<Person>.Property(x => x.Email), pm => { }));
			mapper.ComposedId(map => map.ManyToOne(For<Person>.Property(x => x.User), pm => { }));

			var hbmClass = mapdoc.RootClasses[0];
			Assert.That(hbmClass.Id, Is.Null);
			var hbmCompositeId = hbmClass.CompositeId;
			Assert.That(hbmCompositeId.Items, Has.Length.EqualTo(2));
		}
	}
}