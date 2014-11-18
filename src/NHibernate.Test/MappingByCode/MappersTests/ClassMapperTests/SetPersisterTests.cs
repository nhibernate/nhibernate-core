using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode.Impl;
using NHibernate.Persister.Entity;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.MappersTests.ClassMapperTests
{
	public class SetPersisterTests
	{
		private class EntitySimple
		{
			public int Id { get; set; }
		}

		[Test]
		public void CanSetPersister()
		{
			var mapdoc = new HbmMapping();
			var rc = new ClassMapper(typeof(EntitySimple), mapdoc, For<EntitySimple>.Property(x => x.Id));
			rc.Persister<SingleTableEntityPersister>();
			Assert.That(mapdoc.RootClasses[0].Persister, Is.StringContaining("SingleTableEntityPersister"));
		}
	}
}