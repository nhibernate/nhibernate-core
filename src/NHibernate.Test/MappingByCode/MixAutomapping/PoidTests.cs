using NHibernate.Mapping.ByCode;
using NUnit.Framework;

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
			Assert.That(inspector.IsPersistentId(typeof(MyClass).GetProperty("EntityIdentificator")), Is.True);
		}

		[Test]
		public void WhenNotExplicitlyDeclaredMatchDefaultDelegate()
		{
			var autoinspector = new SimpleModelInspector();

			var inspector = (IModelInspector)autoinspector;
			Assert.That(inspector.IsPersistentId(typeof(TestEntity).GetProperty("Id")), Is.True);
			Assert.That(inspector.IsPersistentId(typeof(TestEntity).GetProperty("id")), Is.True);
			Assert.That(inspector.IsPersistentId(typeof(TestEntity).GetProperty("PoId")), Is.True);
			Assert.That(inspector.IsPersistentId(typeof(TestEntity).GetProperty("POID")), Is.True);
			Assert.That(inspector.IsPersistentId(typeof(TestEntity).GetProperty("OId")), Is.True);
			Assert.That(inspector.IsPersistentId(typeof(TestEntity).GetProperty("TestEntityId")), Is.True);
		}

		[Test]
		public void WhenNotExplicitlyDeclaredThenNoMatchPropertiesOutOfDefaultDelegate()
		{
			var autoinspector = new SimpleModelInspector();

			var inspector = (IModelInspector)autoinspector;
			Assert.That(inspector.IsPersistentId(typeof(TestEntity).GetProperty("testEntityId")), Is.False);
			Assert.That(inspector.IsPersistentId(typeof(TestEntity).GetProperty("Something")), Is.False);
		}
	}
}