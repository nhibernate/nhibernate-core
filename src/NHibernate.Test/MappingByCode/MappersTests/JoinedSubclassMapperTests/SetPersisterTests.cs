using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode.Impl;
using NHibernate.Persister.Entity;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode.MappersTests.JoinedSubclassMapperTests
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
			var rc = new JoinedSubclassMapper(typeof(InheritedSimple), mapdoc);
			rc.Persister<JoinedSubclassEntityPersister>();
			mapdoc.JoinedSubclasses[0].Persister.Should().Contain("JoinedSubclassEntityPersister");
		}
	}
}