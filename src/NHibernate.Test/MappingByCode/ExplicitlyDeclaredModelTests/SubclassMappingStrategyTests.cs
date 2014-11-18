using NHibernate.Mapping.ByCode;
using NUnit.Framework;

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

			Assert.That(inspector.IsTablePerClass(typeof(Inherited1)), Is.False);
			Assert.That(inspector.IsTablePerClassHierarchy(typeof(Inherited1)), Is.True);
			Assert.That(inspector.IsTablePerConcreteClass(typeof(Inherited1)), Is.False);
		}

		[Test]
		public void WhenRegisteredAsDeepSubclassThenIsRegistered()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof(MyClass));
			inspector.AddAsTablePerClassHierarchyEntity(typeof(Inherited2));

			Assert.That(inspector.IsTablePerClass(typeof(Inherited2)), Is.False);
			Assert.That(inspector.IsTablePerClassHierarchy(typeof(Inherited2)), Is.True);
			Assert.That(inspector.IsTablePerConcreteClass(typeof(Inherited2)), Is.False);
		}

		[Test]
		public void WhenRegisteredAsSubclassThenCantRegisterAsJoinedSubclass()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof(MyClass));
			inspector.AddAsTablePerClassHierarchyEntity(typeof(Inherited1));

			Assert.That(() =>
			{
				inspector.AddAsTablePerClassEntity(typeof(Inherited1));
				inspector.IsTablePerClass(typeof(Inherited1));
			}, Throws.TypeOf<MappingException>());
		}

		[Test]
		public void WhenRegisteredAsSubclassThenCantRegisterAsUnionSubclass()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof (MyClass));
			inspector.AddAsTablePerClassHierarchyEntity(typeof (Inherited1));

			Assert.That(() =>
			{
				inspector.AddAsTablePerConcreteClassEntity(typeof (Inherited1));
				inspector.IsTablePerClassHierarchy(typeof (Inherited1));
			}, Throws.TypeOf<MappingException>());
		}

		[Test]
		public void WhenRegisteredAsSubclassThenIsEntity()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof(MyClass));
			inspector.AddAsTablePerClassHierarchyEntity(typeof(Inherited1));

			Assert.That(inspector.IsEntity(typeof(Inherited1)), Is.True);
		}
	}
}