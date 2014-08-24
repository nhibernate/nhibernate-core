using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode.ExplicitlyDeclaredModelTests
{
	public class UnionSubclassMappingStrategyTests
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
		public void WhenRegisteredAsUnionSubclassThenIsRegistered()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof(MyClass));
			inspector.AddAsTablePerConcreteClassEntity(typeof(Inherited1));

			inspector.IsTablePerClass(typeof(Inherited1)).Should().Be.False();
			inspector.IsTablePerClassHierarchy(typeof(Inherited1)).Should().Be.False();
			inspector.IsTablePerConcreteClass(typeof(Inherited1)).Should().Be.True();
		}

		[Test]
		public void WhenRegisteredAsDeepUnionSubclassThenIsRegistered()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof(MyClass));
			inspector.AddAsTablePerConcreteClassEntity(typeof(Inherited2));

			inspector.IsTablePerClass(typeof(Inherited2)).Should().Be.False();
			inspector.IsTablePerClassHierarchy(typeof(Inherited2)).Should().Be.False();
			inspector.IsTablePerConcreteClass(typeof(Inherited2)).Should().Be.True();
		}

		[Test]
		public void WhenRegisteredAsUnionSubclassThenCantRegisterAsSubclass()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof (MyClass));
			inspector.AddAsTablePerConcreteClassEntity(typeof (Inherited1));

			Executing.This(() =>
			               {
			               	inspector.AddAsTablePerClassHierarchyEntity(typeof (Inherited1));
			               	inspector.IsTablePerConcreteClass(typeof (Inherited1));
			               }).Should().Throw<MappingException>();
		}

		[Test]
		public void WhenRegisteredAsUnionSubclassThenCantRegisterAsJoinedSubclass()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof (MyClass));
			inspector.AddAsTablePerConcreteClassEntity(typeof (Inherited1));

			Executing.This(() =>
			               {
			               	inspector.AddAsTablePerClassEntity(typeof (Inherited1));
			               	inspector.IsTablePerConcreteClass(typeof (Inherited1));
			               }).Should().Throw<MappingException>();
		}

		[Test]
		public void WhenRegisteredAsUnionSubclassThenIsEntity()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof(MyClass));
			inspector.AddAsTablePerConcreteClassEntity(typeof(Inherited1));

			inspector.IsEntity(typeof(Inherited1)).Should().Be.True();
		}
	}
}