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
			inspector.IsTablePerConcreteClass(typeof(Inherited2)).Should().Be.False();
		}

		[Test]
		public void WhenRegisteredAsSubclassThenCantRegisterAsJoinedSubclass()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof(MyClass));
			inspector.AddAsTablePerClassHierarchyEntity(typeof(Inherited1));

			Executing.This(() =>
										 {
											 inspector.AddAsTablePerClassEntity(typeof(Inherited1));
											 inspector.IsTablePerClass(typeof(Inherited1));
										 }).Should().Throw<MappingException>();
		}

		[Test]
		public void WhenRegisteredAsSubclassThenCantRegisterAsUnionSubclass()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof (MyClass));
			inspector.AddAsTablePerClassHierarchyEntity(typeof (Inherited1));

			Executing.This(() =>
			               {
			               	inspector.AddAsTablePerConcreteClassEntity(typeof (Inherited1));
			               	inspector.IsTablePerClassHierarchy(typeof (Inherited1));
			               }).Should().Throw<MappingException>();
		}

		[Test]
		public void WhenRegisteredAsSubclassThenIsEntity()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof(MyClass));
			inspector.AddAsTablePerClassHierarchyEntity(typeof(Inherited1));

			inspector.IsEntity(typeof(Inherited1)).Should().Be.True();
		}
	}
}