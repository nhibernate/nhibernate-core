using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode.MixAutomapping
{
	public class EntityTests
	{
		private class AComponent
		{
			public string S { get; set; }
		}
		private class AEntity
		{
			public int Id { get; set; }
		}
		private class Entity
		{
			private int id;
		}

		private enum Something
		{

		}

		[Test]
		public void WhenAClassIsExplicitlyDeclaredAsEntityThenIsEntity()
		{
			var autoinspector = new SimpleModelInspector();
			var mapper = new ModelMapper(autoinspector);
			mapper.Class<AComponent>(map => { });

			var inspector = (IModelInspector)autoinspector;
			inspector.IsEntity(typeof(AComponent)).Should().Be.True();
		}

		[Test]
		public void ClassWithPoidIsEntity()
		{
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;
			inspector.IsEntity(typeof(AEntity)).Should().Be.True();
		}

		[Test]
		public void ClassWithoutPoidIsNotEntity()
		{
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;
			inspector.IsEntity(typeof(AComponent)).Should().Be.False();
		}

		[Test]
		public void ClassWithPoidFieldIsEntity()
		{
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;
			inspector.IsEntity(typeof(Entity)).Should().Be.True();
		}

		[Test]
		public void EnumIsNotEntity()
		{
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;
			inspector.IsEntity(typeof(Something)).Should().Be.False();
		}

		[Test]
		public void StringIsNotEntity()
		{
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;
			inspector.IsEntity(typeof(string)).Should().Be.False();
		}
	}
}