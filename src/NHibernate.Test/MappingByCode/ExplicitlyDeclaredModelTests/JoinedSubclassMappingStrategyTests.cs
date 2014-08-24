using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode.ExplicitlyDeclaredModelTests
{
	public class JoinedSubclassMappingStrategyTests
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
		public void WhenRegisteredAsJoinedSubclassThenIsRegistered()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof(MyClass));
			inspector.AddAsTablePerClassEntity(typeof(Inherited1));

			inspector.IsTablePerClass(typeof(Inherited1)).Should().Be.True();
			inspector.IsTablePerClassHierarchy(typeof(Inherited1)).Should().Be.False();
			inspector.IsTablePerConcreteClass(typeof(Inherited1)).Should().Be.False();
		}

		[Test]
		public void WhenRegisteredAsDeppJoinedSubclassThenIsRegistered()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof(MyClass));
			inspector.AddAsTablePerClassEntity(typeof(Inherited2));

			inspector.IsTablePerClass(typeof(Inherited2)).Should().Be.True();
			inspector.IsTablePerClassHierarchy(typeof(Inherited2)).Should().Be.False();
			inspector.IsTablePerConcreteClass(typeof(Inherited2)).Should().Be.False();
		}

		[Test]
		public void WhenRegisteredAsJoinedSubclassThenCantRegisterAsSubclass()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof (MyClass));
			inspector.AddAsTablePerClassEntity(typeof (Inherited1));

			Executing.This(() =>
			               {
			               	inspector.AddAsTablePerClassHierarchyEntity(typeof (Inherited1));
			               	inspector.IsTablePerClass(typeof (Inherited1));
			               }).Should().Throw<MappingException>();
		}

		[Test]
		public void WhenRegisteredAsJoinedSubclassThenCantRegisterAsUnionSubclass()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof (MyClass));
			inspector.AddAsTablePerClassEntity(typeof (Inherited1));

			Executing.This(() =>
			               {
			               	inspector.AddAsTablePerConcreteClassEntity(typeof (Inherited1));
			               	inspector.IsTablePerClass(typeof (Inherited1));
			               }).Should().Throw<MappingException>();
		}

		[Test]
		public void WhenRegisteredAsJoinedSubclassThenIsEntity()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof(MyClass));
			inspector.AddAsTablePerClassEntity(typeof(Inherited1));

			inspector.IsEntity(typeof(Inherited1)).Should().Be.True();
		}
	}
}