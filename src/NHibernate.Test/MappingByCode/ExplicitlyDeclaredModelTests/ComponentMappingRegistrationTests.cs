using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;

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

			inspector.IsComponent(typeof(MyComponent)).Should().Be.True();
		}

		[Test]
		public void WhenRegisteredAsEntityThenCantRegisterAsComponent()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof(MyComponent));

			inspector.Executing(x => x.AddAsComponent(typeof(MyComponent))).Throws<MappingException>();
		}

		[Test]
		public void WhenRegisteredAsComponetThenCantRegisterAsRootEntity()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsComponent(typeof(MyComponent));

			inspector.Executing(x => x.AddAsRootEntity(typeof(MyComponent))).Throws<MappingException>();
		}

		[Test]
		public void WhenRegisteredAsComponetThenCantRegisterAsJoinedSubclass()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsComponent(typeof(MyComponent));

			inspector.Executing(x => x.AddAsTablePerClassEntity(typeof(MyComponent))).Throws<MappingException>();
		}

		[Test]
		public void WhenRegisteredAsComponetThenCantRegisterAsSubclass()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsComponent(typeof(MyComponent));

			inspector.Executing(x => x.AddAsTablePerClassHierarchyEntity(typeof(MyComponent))).Throws<MappingException>();
		}

		[Test]
		public void WhenRegisteredAsComponetThenCantRegisterAsUnionSubclass()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsComponent(typeof(MyComponent));

			inspector.Executing(x => x.AddAsTablePerConcreteClassEntity(typeof(MyComponent))).Throws<MappingException>();
		}
	}
}