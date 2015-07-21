using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode.Impl;
using NHibernate.Persister.Entity;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.MappersTests.UnionSubclassMapperTests
{
	public class SetPersisterTests
	{
		private class EntitySimple
		{
			public int Id { get; set; }
		}

		private class InheritedSimple : EntitySimple
		{
		}

		[Test]
		public void CanSetPersister()
		{
			var mapdoc = new HbmMapping();
			var rc = new UnionSubclassMapper(typeof(InheritedSimple), mapdoc);
			rc.Persister<UnionSubclassEntityPersister>();
			Assert.That(mapdoc.UnionSubclasses[0].Persister, Is.StringContaining("UnionSubclassEntityPersister"));
		}
	}
}