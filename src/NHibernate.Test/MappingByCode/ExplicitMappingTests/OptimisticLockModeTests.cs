using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode.ExplicitMappingTests
{
	[TestFixture]
	public class OptimisticLockModeTests
	{
		private class MyClass
		{
			public int Id { get; set; }
			public int Version { get; set; }
		}

		[Test]
		public void OptimisticLockModeTest()
		{
			//NH-2823
			var mapper = new ModelMapper();
			mapper.Class<MyClass>(
				map =>
				{
					map.Id(x => x.Id, idmap => { });
					map.OptimisticLock(OptimisticLockMode.Dirty);
				});

			var mappings = mapper.CompileMappingForAllExplicitlyAddedEntities();
			Assert.AreEqual(mappings.RootClasses[0].optimisticlock, HbmOptimisticLockMode.Dirty);
		}
	}
}