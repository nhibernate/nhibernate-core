using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;

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

			inspector.IsTablePerClass(typeof(MyClass)).Should().Be.True();
			inspector.IsTablePerClass(typeof(Inherited)).Should().Be.True();
		}

		[Test]
		public void WhenExplicitlyDeclaredAsSubclassThenIsNotTablePerClass()
		{
			var autoinspector = new SimpleModelInspector();
			var mapper = new ModelMapper(autoinspector);
			mapper.Class<MyClass>(x => { });
			mapper.Subclass<Inherited>(x => { });

			var inspector = (IModelInspector)autoinspector;

			inspector.IsTablePerClass(typeof(MyClass)).Should().Be.False();
			inspector.IsTablePerClass(typeof(Inherited)).Should().Be.False();
		}

		[Test]
		public void WhenExplicitlyDeclaredAsUnionSubclassThenIsNotTablePerClass()
		{
			var autoinspector = new SimpleModelInspector();
			var mapper = new ModelMapper(autoinspector);
			mapper.Class<MyClass>(x => { });
			mapper.UnionSubclass<Inherited>(x => { });

			var inspector = (IModelInspector)autoinspector;

			inspector.IsTablePerClass(typeof(MyClass)).Should().Be.False();
			inspector.IsTablePerClass(typeof(Inherited)).Should().Be.False();
		}
	}
}