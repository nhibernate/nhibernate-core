using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode.Impl;
using NHibernate.Test.MappingByCode;
using NUnit.Framework;
using SharpTestsEx;


namespace NHibernate.Test.NHSpecificTest.NH2989
{
	[TestFixture]
	public class Fixture
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
		public void WhenComponentAsIdMapperCreatedThenSetTheComponentName()
		{
			var mapdoc = new HbmMapping();
			var component = new HbmCompositeId();
			new ComponentAsIdMapper(typeof(PersonId), For<Person>.Property(x => x.Id), component, mapdoc);

			component.name.Should().Be(For<Person>.Property(x => x.Id).Name);
		}
	}
}