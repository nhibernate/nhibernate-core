using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.MixAutomapping
{
	[TestFixture]
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
			// Used by reflection
#pragma warning disable CS0169 // The field is never used
			private int id;
#pragma warning restore CS0169 // The field is never used
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
			Assert.That(inspector.IsEntity(typeof(AComponent)), Is.True);
		}

		[Test]
		public void ClassWithPoidIsEntity()
		{
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;
			Assert.That(inspector.IsEntity(typeof(AEntity)), Is.True);
		}

		[Test]
		public void ClassWithoutPoidIsNotEntity()
		{
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;
			Assert.That(inspector.IsEntity(typeof(AComponent)), Is.False);
		}

		[Test]
		public void ClassWithPoidFieldIsEntity()
		{
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;
			Assert.That(inspector.IsEntity(typeof(Entity)), Is.True);
		}

		[Test]
		public void EnumIsNotEntity()
		{
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;
			Assert.That(inspector.IsEntity(typeof(Something)), Is.False);
		}

		[Test]
		public void StringIsNotEntity()
		{
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;
			Assert.That(inspector.IsEntity(typeof(string)), Is.False);
		}
	}
}
