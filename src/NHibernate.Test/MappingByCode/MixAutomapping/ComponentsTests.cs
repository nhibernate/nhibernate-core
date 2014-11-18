using NHibernate.Mapping.ByCode;
using NUnit.Framework;

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
			Assert.That(inspector.IsComponent(typeof(AEntity)), Is.True);
		}

		[Test]
		public void ClassWithoutPoidIsComponent()
		{
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;
			Assert.That(inspector.IsComponent(typeof(AComponent)), Is.True);
		}

		[Test]
		public void ClassWithPoidIsNotComponent()
		{
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;
			Assert.That(inspector.IsComponent(typeof(AEntity)), Is.False);
		}

		[Test]
		public void ClassWithPoidFieldIsNotComponent()
		{
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;
			Assert.That(inspector.IsComponent(typeof(Entity)), Is.False);
		}

		[Test]
		public void EnumIsNotComponent()
		{
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;
			Assert.That(inspector.IsComponent(typeof(Something)), Is.False);
		}

		[Test]
		public void StringIsNotComponent()
		{
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;
			Assert.That(inspector.IsComponent(typeof(string)), Is.False);
		}
	}
}