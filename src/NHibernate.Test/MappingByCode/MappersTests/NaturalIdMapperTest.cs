using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.MappersTests
{
	public class NaturalIdMapperTest
	{
		private class EntitySimpleWithNaturalId
		{
			public string Something { get; set; }
		}

		[Test]
		public void CanSetMutable()
		{
			var mapdoc = new HbmMapping();
			var hbmClass = new HbmClass();
			var nid = new NaturalIdMapper(typeof(EntitySimpleWithNaturalId), hbmClass, mapdoc);
			// to have the natural-id assigned ot must have at least a property
			nid.Property(For<EntitySimpleWithNaturalId>.Property(x => x.Something), pm => { });

			var hbmNaturalId = hbmClass.naturalid;
			nid.Mutable(true);
			Assert.That(hbmNaturalId.mutable, Is.True);
		}
	}
}