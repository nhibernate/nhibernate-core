using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode.Impl;
using NHibernate.Persister.Entity;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.MappersTests.SubclassMapperTests
{
	public class SetPersisterTests
	{
		private class EntitySimple
		{
			public int Id { get; set; }
		}

		private class HineritedSimple: EntitySimple
		{
		}

		[Test]
		public void CanSetPersister()
		{
			var mapdoc = new HbmMapping();
			var rc = new SubclassMapper(typeof(HineritedSimple), mapdoc);
			rc.Persister<SingleTableEntityPersister>();
			Assert.That(mapdoc.SubClasses[0].Persister, Is.StringContaining("SingleTableEntityPersister"));
		}
	}
}