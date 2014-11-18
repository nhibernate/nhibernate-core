using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.MixAutomapping
{
	public class DefaultClassHierarchyRepresentationTests
	{
		private class MyClass
		{
			public int Id { get; set; }
		}

		private class Inherited: MyClass
		{
		}

		[Test]
		public void WhenNotExplicitlyDeclaredThenIsTablePerClass()
		{
			var autoinspector = new SimpleModelInspector();
			var mapper = new ModelMapper(autoinspector);
			mapper.Class<MyClass>(x => { });
			var inspector = (IModelInspector)autoinspector;

			Assert.That(inspector.IsTablePerClass(typeof(MyClass)), Is.True);
			Assert.That(inspector.IsTablePerClass(typeof(Inherited)), Is.True);
		}

		[Test]
		public void WhenExplicitlyDeclaredAsSubclassThenIsNotTablePerClass()
		{
			var autoinspector = new SimpleModelInspector();
			var mapper = new ModelMapper(autoinspector);
			mapper.Class<MyClass>(x => { });
			mapper.Subclass<Inherited>(x => { });

			var inspector = (IModelInspector)autoinspector;

			Assert.That(inspector.IsTablePerClass(typeof(MyClass)), Is.False);
			Assert.That(inspector.IsTablePerClass(typeof(Inherited)), Is.False);
		}

		[Test]
		public void WhenExplicitlyDeclaredAsUnionSubclassThenIsNotTablePerClass()
		{
			var autoinspector = new SimpleModelInspector();
			var mapper = new ModelMapper(autoinspector);
			mapper.Class<MyClass>(x => { });
			mapper.UnionSubclass<Inherited>(x => { });

			var inspector = (IModelInspector)autoinspector;

			Assert.That(inspector.IsTablePerClass(typeof(MyClass)), Is.False);
			Assert.That(inspector.IsTablePerClass(typeof(Inherited)), Is.False);
		}
	}
}