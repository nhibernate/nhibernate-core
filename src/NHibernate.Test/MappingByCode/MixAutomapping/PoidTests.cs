using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode.MixAutomapping
{
	public class PoidTests
	{
		private class MyClass
		{
			public int EntityIdentificator { get; set; }
		}
		private class TestEntity
		{
			public int Id { get; set; }
			public int id { get; set; }
			public int PoId { get; set; }
			public int POID { get; set; }
			public int OId { get; set; }
			public int TestEntityId { get; set; }
			public int testEntityId { get; set; }
			public int Something { get; set; }
		}

		[Test]
		public void WhenExplicitDeclaredThenMatch()
		{
			var autoinspector = new SimpleModelInspector();
			var mapper = new ModelMapper(autoinspector);
			mapper.Class<MyClass>(map => map.Id(x => x.EntityIdentificator));

			var inspector = (IModelInspector)autoinspector;
			inspector.IsPersistentId(typeof(MyClass).GetProperty("EntityIdentificator")).Should().Be.True();
		}

		[Test]
		public void WhenNotExplicitlyDeclaredMatchDefaultDelegate()
		{
			var autoinspector = new SimpleModelInspector();

			var inspector = (IModelInspector)autoinspector;
			inspector.IsPersistentId(typeof(TestEntity).GetProperty("Id")).Should().Be.True();
			inspector.IsPersistentId(typeof(TestEntity).GetProperty("id")).Should().Be.True();
			inspector.IsPersistentId(typeof(TestEntity).GetProperty("PoId")).Should().Be.True();
			inspector.IsPersistentId(typeof(TestEntity).GetProperty("POID")).Should().Be.True();
			inspector.IsPersistentId(typeof(TestEntity).GetProperty("OId")).Should().Be.True();
			inspector.IsPersistentId(typeof(TestEntity).GetProperty("TestEntityId")).Should().Be.True();
		}

		[Test]
		public void WhenNotExplicitlyDeclaredThenNoMatchPropertiesOutOfDefaultDelegate()
		{
			var autoinspector = new SimpleModelInspector();

			var inspector = (IModelInspector)autoinspector;
			inspector.IsPersistentId(typeof(TestEntity).GetProperty("testEntityId")).Should().Be.False();
			inspector.IsPersistentId(typeof(TestEntity).GetProperty("Something")).Should().Be.False();
		}
	}
}