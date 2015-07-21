using NHibernate.Mapping.ByCode;
using NUnit.Framework;

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
			Assert.That(inspector.IsTablePerClass(typeof(MyClass)), Is.False);
			Assert.That(inspector.IsTablePerClassHierarchy(typeof(MyClass)), Is.False);
			Assert.That(inspector.IsTablePerConcreteClass(typeof(MyClass)), Is.False);
		}

		[Test]
		public void WhenRegisteredJoinedSubclassThenTheStrategyIsDefinedEvenForRoot()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof(MyClass));
			inspector.AddAsTablePerClassEntity(typeof(Inherited1));
			Assert.That(inspector.IsTablePerClass(typeof(MyClass)), Is.True);
			Assert.That(inspector.IsTablePerClassHierarchy(typeof(MyClass)), Is.False);
			Assert.That(inspector.IsTablePerConcreteClass(typeof(MyClass)), Is.False);
		}

		[Test]
		public void WhenRegisteredJoinedDeepSubclassThenTheStrategyIsDefinedEvenForRoot()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof(MyClass));
			inspector.AddAsTablePerClassEntity(typeof(Inherited2));
			Assert.That(inspector.IsTablePerClass(typeof(MyClass)), Is.True);
			Assert.That(inspector.IsTablePerClassHierarchy(typeof(MyClass)), Is.False);
			Assert.That(inspector.IsTablePerConcreteClass(typeof(MyClass)), Is.False);
		}

		[Test]
		public void WhenRegisteredSubclassThenTheStrategyIsDefinedEvenForRoot()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof(MyClass));
			inspector.AddAsTablePerClassHierarchyEntity(typeof(Inherited1));
			Assert.That(inspector.IsTablePerClass(typeof(MyClass)), Is.False);
			Assert.That(inspector.IsTablePerClassHierarchy(typeof(MyClass)), Is.True);
			Assert.That(inspector.IsTablePerConcreteClass(typeof(MyClass)), Is.False);
		}

		[Test]
		public void WhenRegisteredDeepSubclassThenTheStrategyIsDefinedEvenForRoot()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof(MyClass));
			inspector.AddAsTablePerClassHierarchyEntity(typeof(Inherited2));

			Assert.That(inspector.IsTablePerClass(typeof(MyClass)), Is.False);
			Assert.That(inspector.IsTablePerClassHierarchy(typeof(MyClass)), Is.True);
			Assert.That(inspector.IsTablePerConcreteClass(typeof(MyClass)), Is.False);
		}


		[Test]
		public void WhenRegisteredConcreteClassThenTheStrategyIsDefinedEvenForRoot()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof(MyClass));
			inspector.AddAsTablePerConcreteClassEntity(typeof(Inherited1));

			Assert.That(inspector.IsTablePerClass(typeof(MyClass)), Is.False);
			Assert.That(inspector.IsTablePerClassHierarchy(typeof(MyClass)), Is.False);
			Assert.That(inspector.IsTablePerConcreteClass(typeof(MyClass)), Is.True);
		}

		[Test]
		public void WhenRegisteredDeepConcreteClassThenTheStrategyIsDefinedEvenForRoot()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof(MyClass));
			inspector.AddAsTablePerConcreteClassEntity(typeof(Inherited2));

			Assert.That(inspector.IsTablePerClass(typeof(MyClass)), Is.False);
			Assert.That(inspector.IsTablePerClassHierarchy(typeof(MyClass)), Is.False);
			Assert.That(inspector.IsTablePerConcreteClass(typeof(MyClass)), Is.True);
		}

		[Test]
		public void WhenRegisteredAsRootThenCantRegisterAsSubclass()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof (MyClass));
			Assert.That(() =>
			{
				inspector.AddAsTablePerClassHierarchyEntity(typeof (MyClass));
				inspector.IsTablePerClassHierarchy(typeof (MyClass));
			}, Throws.TypeOf<MappingException>());
		}

		[Test]
		public void WhenRegisteredAsRootThenCantRegisterAsJoinedSubclass()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof (MyClass));
			Assert.That(() =>
			{
				inspector.AddAsTablePerClassEntity(typeof (MyClass));
				inspector.IsTablePerClass(typeof (MyClass));
			}, Throws.TypeOf<MappingException>());
		}

		[Test]
		public void WhenRegisteredAsRootThenCantRegisterAsUnionSubclass()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof (MyClass));
			Assert.That(() =>
			{
				inspector.AddAsTablePerConcreteClassEntity(typeof (MyClass));
				inspector.IsTablePerClass(typeof (MyClass));
			}, Throws.TypeOf<MappingException>());
		}

		[Test]
		public void WhenRegisteredAsRootThenIsEntity()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof(MyClass));

			Assert.That(inspector.IsEntity(typeof(MyClass)), Is.True);
		}

		[Test]
		public void WhenMultipleRootRegisteredThenThrowsMappingException()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof(MyClass));
			inspector.AddAsRootEntity(typeof(Inherited1));
			Assert.That(()=>
			{
				inspector.AddAsTablePerClassEntity(typeof(Inherited2));
				inspector.IsTablePerClass(typeof(Inherited2));
			}, Throws.TypeOf<MappingException>());
		}
	}
}