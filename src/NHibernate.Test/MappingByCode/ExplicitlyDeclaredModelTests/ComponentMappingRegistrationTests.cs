using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.ExplicitlyDeclaredModelTests
{
	public class ComponentMappingRegistrationTests
	{
		private class MyComponent
		{
			
		}

		[Test]
		public void WhenRegisteredAsComponentThenIsRegistered()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsComponent(typeof(MyComponent));

			Assert.That(inspector.IsComponent(typeof(MyComponent)), Is.True);
		}

		[Test]
		public void WhenRegisteredAsEntityThenCantRegisterAsComponent()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof(MyComponent));

			Assert.That(() => inspector.AddAsComponent(typeof(MyComponent)), Throws.TypeOf<MappingException>());
		}

		[Test]
		public void WhenRegisteredAsComponetThenCantRegisterAsRootEntity()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsComponent(typeof(MyComponent));

			Assert.That(() => inspector.AddAsRootEntity(typeof(MyComponent)), Throws.TypeOf<MappingException>());
		}

		[Test]
		public void WhenRegisteredAsComponetThenCantRegisterAsJoinedSubclass()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsComponent(typeof (MyComponent));

			Assert.That(() =>
			{
				inspector.AddAsTablePerClassEntity(typeof (MyComponent));
				inspector.IsTablePerClass(typeof (MyComponent));
			}, Throws.TypeOf<MappingException>());
		}

		[Test]
		public void WhenRegisteredAsComponetThenCantRegisterAsSubclass()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsComponent(typeof (MyComponent));

			Assert.That(() =>
			{
				inspector.AddAsTablePerClassHierarchyEntity(typeof (MyComponent));
				inspector.IsTablePerClassHierarchy(typeof (MyComponent));
			}, Throws.TypeOf<MappingException>());
		}

		[Test]
		public void WhenRegisteredAsComponetThenCantRegisterAsUnionSubclass()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsComponent(typeof (MyComponent));

			Assert.That(() =>
			{
				inspector.AddAsTablePerConcreteClassEntity(typeof (MyComponent));
				inspector.IsTablePerConcreteClass(typeof (MyComponent));
			}, Throws.TypeOf<MappingException>());
		}
	}
}