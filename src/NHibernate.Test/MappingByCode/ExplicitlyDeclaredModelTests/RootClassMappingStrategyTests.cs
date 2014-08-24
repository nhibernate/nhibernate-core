using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode.ExplicitlyDeclaredModelTests
{
	public class RootClassMappingStrategyTests
	{
		private class MyClass
		{
			
		}
		private class Inherited1: MyClass
		{

		}
		private class Inherited2 : Inherited1
		{

		}

		[Test]
		public void WhenRegisteredAsRootThenDoesNotRegisterTheStrategy()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof(MyClass));
			inspector.IsTablePerClass(typeof(MyClass)).Should().Be.False();
			inspector.IsTablePerClassHierarchy(typeof(MyClass)).Should().Be.False();
			inspector.IsTablePerConcreteClass(typeof(MyClass)).Should().Be.False();
		}

		[Test]
		public void WhenRegisteredJoinedSubclassThenTheStrategyIsDefinedEvenForRoot()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof(MyClass));
			inspector.AddAsTablePerClassEntity(typeof(Inherited1));
			inspector.IsTablePerClass(typeof(MyClass)).Should().Be.True();
			inspector.IsTablePerClassHierarchy(typeof(MyClass)).Should().Be.False();
			inspector.IsTablePerConcreteClass(typeof(MyClass)).Should().Be.False();
		}

		[Test]
		public void WhenRegisteredJoinedDeepSubclassThenTheStrategyIsDefinedEvenForRoot()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof(MyClass));
			inspector.AddAsTablePerClassEntity(typeof(Inherited2));
			inspector.IsTablePerClass(typeof(MyClass)).Should().Be.True();
			inspector.IsTablePerClassHierarchy(typeof(MyClass)).Should().Be.False();
			inspector.IsTablePerConcreteClass(typeof(MyClass)).Should().Be.False();
		}

		[Test]
		public void WhenRegisteredSubclassThenTheStrategyIsDefinedEvenForRoot()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof(MyClass));
			inspector.AddAsTablePerClassHierarchyEntity(typeof(Inherited1));
			inspector.IsTablePerClass(typeof(MyClass)).Should().Be.False();
			inspector.IsTablePerClassHierarchy(typeof(MyClass)).Should().Be.True();
			inspector.IsTablePerConcreteClass(typeof(MyClass)).Should().Be.False();
		}

		[Test]
		public void WhenRegisteredDeepSubclassThenTheStrategyIsDefinedEvenForRoot()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof(MyClass));
			inspector.AddAsTablePerClassHierarchyEntity(typeof(Inherited2));

			inspector.IsTablePerClass(typeof(MyClass)).Should().Be.False();
			inspector.IsTablePerClassHierarchy(typeof(MyClass)).Should().Be.True();
			inspector.IsTablePerConcreteClass(typeof(MyClass)).Should().Be.False();
		}


		[Test]
		public void WhenRegisteredConcreteClassThenTheStrategyIsDefinedEvenForRoot()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof(MyClass));
			inspector.AddAsTablePerConcreteClassEntity(typeof(Inherited1));

			inspector.IsTablePerClass(typeof(MyClass)).Should().Be.False();
			inspector.IsTablePerClassHierarchy(typeof(MyClass)).Should().Be.False();
			inspector.IsTablePerConcreteClass(typeof(MyClass)).Should().Be.True();
		}

		[Test]
		public void WhenRegisteredDeepConcreteClassThenTheStrategyIsDefinedEvenForRoot()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof(MyClass));
			inspector.AddAsTablePerConcreteClassEntity(typeof(Inherited2));

			inspector.IsTablePerClass(typeof(MyClass)).Should().Be.False();
			inspector.IsTablePerClassHierarchy(typeof(MyClass)).Should().Be.False();
			inspector.IsTablePerConcreteClass(typeof(MyClass)).Should().Be.True();
		}

		[Test]
		public void WhenRegisteredAsRootThenCantRegisterAsSubclass()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof (MyClass));
			Executing.This(() =>
			               {
			               	inspector.AddAsTablePerClassHierarchyEntity(typeof (MyClass));
			               	inspector.IsTablePerClassHierarchy(typeof (MyClass));
			               }).Should().Throw<MappingException>();
		}

		[Test]
		public void WhenRegisteredAsRootThenCantRegisterAsJoinedSubclass()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof (MyClass));
			Executing.This(() =>
			               {
			               	inspector.AddAsTablePerClassEntity(typeof (MyClass));
			               	inspector.IsTablePerClass(typeof (MyClass));
			               }).Should().Throw<MappingException>();
		}

		[Test]
		public void WhenRegisteredAsRootThenCantRegisterAsUnionSubclass()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof (MyClass));
			Executing.This(() =>
			               {
			               	inspector.AddAsTablePerConcreteClassEntity(typeof (MyClass));
			               	inspector.IsTablePerClass(typeof (MyClass));
			               }).Should().Throw<MappingException>();
		}

		[Test]
		public void WhenRegisteredAsRootThenIsEntity()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof(MyClass));

			inspector.IsEntity(typeof(MyClass)).Should().Be.True();
		}

		[Test]
		public void WhenMultipleRootRegisteredThenThrowsMappingException()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof(MyClass));
			inspector.AddAsRootEntity(typeof(Inherited1));
			Executing.This(()=>
			               {
			               	inspector.AddAsTablePerClassEntity(typeof(Inherited2));
											inspector.IsTablePerClass(typeof(Inherited2));
			               }).Should().Throw<MappingException>();
		}
	}
}