using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode.ExplicitlyDeclaredModelTests
{
	public class SubclassMappingStrategyTests
	{
		private class MyClass
		{

		}
		private class Inherited1 : MyClass
		{

		}
		private class Inherited2 : Inherited1
		{

		}

		[Test]
		public void WhenRegisteredAsSubclassThenIsRegistered()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof(MyClass));
			inspector.AddAsTablePerClassHierarchyEntity(typeof(Inherited1));

			inspector.IsTablePerClass(typeof(Inherited1)).Should().Be.False();
			inspector.IsTablePerClassHierarchy(typeof(Inherited1)).Should().Be.True();
			inspector.IsTablePerClassHierarchyJoin(typeof(Inherited1)).Should().Be.False();
			inspector.IsTablePerConcreteClass(typeof(Inherited1)).Should().Be.False();
		}

		[Test]
		public void WhenRegisteredAsDeepSubclassThenIsRegistered()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof(MyClass));
			inspector.AddAsTablePerClassHierarchyEntity(typeof(Inherited2));

			inspector.IsTablePerClass(typeof(Inherited2)).Should().Be.False();
			inspector.IsTablePerClassHierarchy(typeof(Inherited2)).Should().Be.True();
			inspector.IsTablePerClassHierarchyJoin(typeof(Inherited2)).Should().Be.False();
			inspector.IsTablePerConcreteClass(typeof(Inherited2)).Should().Be.False();
		}

		[Test]
		public void WhenRegisteredAsSubclassThenCantRegisterAsJoinedSubclass()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof(MyClass));
			inspector.AddAsTablePerClassHierarchyEntity(typeof(Inherited1));

			inspector.Executing(x => x.AddAsTablePerClassEntity(typeof(Inherited1))).Throws<MappingException>();
		}

		[Test]
		public void WhenRegisteredAsSubclassThenCantRegisterAsSubclassJoin()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof(MyClass));
			inspector.AddAsTablePerClassHierarchyEntity(typeof(Inherited1));

			inspector.Executing(x => x.AddAsTablePerClassHierarchyJoinEntity(typeof(Inherited1))).Throws<MappingException>();
		}

		[Test]
		public void WhenRegisteredAsSubclassThenCantRegisterAsUnionSubclass()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof(MyClass));
			inspector.AddAsTablePerClassHierarchyEntity(typeof(Inherited1));

			inspector.Executing(x => x.AddAsTablePerConcreteClassEntity(typeof(Inherited1))).Throws<MappingException>();
		}

	}
}