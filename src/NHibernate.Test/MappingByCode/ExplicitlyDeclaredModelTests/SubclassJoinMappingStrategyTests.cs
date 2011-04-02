using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode.ExplicitlyDeclaredModelTests
{
	public class SubclassJoinMappingStrategyTests
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
		public void WhenRegisteredAsSubclassJoinThenIsRegistered()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof(MyClass));
			inspector.AddAsTablePerClassHierarchyJoinEntity(typeof(Inherited1));

			inspector.IsTablePerClass(typeof(Inherited1)).Should().Be.False();
			inspector.IsTablePerClassHierarchy(typeof(Inherited1)).Should().Be.True();
			inspector.IsTablePerClassHierarchyJoin(typeof(Inherited1)).Should().Be.True();
			inspector.IsTablePerConcreteClass(typeof(Inherited1)).Should().Be.False();
		}

		[Test]
		public void WhenRegisteredAsDeepSubclassJoinThenIsRegistered()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof(MyClass));
			inspector.AddAsTablePerClassHierarchyJoinEntity(typeof(Inherited2));

			inspector.IsTablePerClass(typeof(Inherited2)).Should().Be.False();
			inspector.IsTablePerClassHierarchy(typeof(Inherited2)).Should().Be.True();
			inspector.IsTablePerClassHierarchyJoin(typeof(Inherited2)).Should().Be.True();
			inspector.IsTablePerConcreteClass(typeof(Inherited2)).Should().Be.False();
		}

		[Test]
		public void WhenRegisteredAsSubclassJoinThenCantRegisterAsJoinedSubclass()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof(MyClass));
			inspector.AddAsTablePerClassHierarchyJoinEntity(typeof(Inherited1));

			inspector.Executing(x => x.AddAsTablePerClassEntity(typeof(Inherited1))).Throws<MappingException>();
		}

		[Test]
		public void WhenRegisteredAsSubclassJoinThenCantRegisterAsSubclass()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof(MyClass));
			inspector.AddAsTablePerClassHierarchyJoinEntity(typeof(Inherited1));

			inspector.Executing(x => x.AddAsTablePerClassHierarchyEntity(typeof(Inherited1))).Throws<MappingException>();
		}

		[Test]
		public void WhenRegisteredAsSubclassJoinThenCantRegisterAsUnionSubclass()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof(MyClass));
			inspector.AddAsTablePerClassHierarchyJoinEntity(typeof(Inherited1));

			inspector.Executing(x => x.AddAsTablePerConcreteClassEntity(typeof(Inherited1))).Throws<MappingException>();
		}
	}
}