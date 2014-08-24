using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;

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

			inspector.IsRootEntity(typeof(Person)).Should().Be.True();
			inspector.IsRootEntity(typeof(Product)).Should().Be.False();
		}

		[Test]
		public void WhenCustomizedThenUseCustomizedPredicate()
		{
			var autoinspector = new SimpleModelInspector();
			autoinspector.IsRootEntity((t, declared) => typeof(BaseEntity).Equals(t.BaseType));

			var inspector = (IModelInspector)autoinspector;

			inspector.IsRootEntity(typeof(Person)).Should().Be.False();
			inspector.IsRootEntity(typeof(Product)).Should().Be.True();
		}
	}
}