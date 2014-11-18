using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.MixAutomapping
{
	public class RootEntityTests
	{
		private class Person
		{
			public int Id { get; set; }
		}

		private class BaseEntity
		{
			public int Id { get; set; }
		}

		private class Product: BaseEntity
		{
		}

		[Test]
		public void ByDefaultInheritedFromObject()
		{
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;

			Assert.That(inspector.IsRootEntity(typeof(Person)), Is.True);
			Assert.That(inspector.IsRootEntity(typeof(Product)), Is.False);
		}

		[Test]
		public void WhenCustomizedThenUseCustomizedPredicate()
		{
			var autoinspector = new SimpleModelInspector();
			autoinspector.IsRootEntity((t, declared) => typeof(BaseEntity).Equals(t.BaseType));

			var inspector = (IModelInspector)autoinspector;

			Assert.That(inspector.IsRootEntity(typeof(Person)), Is.False);
			Assert.That(inspector.IsRootEntity(typeof(Product)), Is.True);
		}
	}
}