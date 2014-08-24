using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode.MixAutomapping
{
	public class ComponentsTests
	{
		// a class without Poid is a Component
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
		public void WhenAClassIsExplicitlyDeclaredAsComponentThenIsComponent()
		{
			var autoinspector = new SimpleModelInspector();
			var mapper = new ModelMapper(autoinspector);
			mapper.Component<AEntity>(map => { });

			var inspector = (IModelInspector)autoinspector;
			inspector.IsComponent(typeof(AEntity)).Should().Be.True();
		}

		[Test]
		public void ClassWithoutPoidIsComponent()
		{
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;
			inspector.IsComponent(typeof(AComponent)).Should().Be.True();
		}

		[Test]
		public void ClassWithPoidIsNotComponent()
		{
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;
			inspector.IsComponent(typeof(AEntity)).Should().Be.False();
		}

		[Test]
		public void ClassWithPoidFieldIsNotComponent()
		{
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;
			inspector.IsComponent(typeof(Entity)).Should().Be.False();
		}

		[Test]
		public void EnumIsNotComponent()
		{
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;
			inspector.IsComponent(typeof(Something)).Should().Be.False();
		}

		[Test]
		public void StringIsNotComponent()
		{
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;
			inspector.IsComponent(typeof(string)).Should().Be.False();
		}
	}
}