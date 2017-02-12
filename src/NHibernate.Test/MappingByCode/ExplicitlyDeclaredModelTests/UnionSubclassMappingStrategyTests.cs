using NHibernate.Mapping.ByCode;
using NUnit.Framework;

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

			Assert.That(inspector.IsTablePerClass(typeof(Inherited1)), Is.False);
			Assert.That(inspector.IsTablePerClassHierarchy(typeof(Inherited1)), Is.False);
			Assert.That(inspector.IsTablePerConcreteClass(typeof(Inherited1)), Is.True);
		}

		[Test]
		public void WhenRegisteredAsDeepUnionSubclassThenIsRegistered()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof(MyClass));
			inspector.AddAsTablePerConcreteClassEntity(typeof(Inherited2));

			Assert.That(inspector.IsTablePerClass(typeof(Inherited2)), Is.False);
			Assert.That(inspector.IsTablePerClassHierarchy(typeof(Inherited2)), Is.False);
			Assert.That(inspector.IsTablePerConcreteClass(typeof(Inherited2)), Is.True);
		}

		[Test]
		public void WhenRegisteredAsUnionSubclassThenCantRegisterAsSubclass()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof (MyClass));
			inspector.AddAsTablePerConcreteClassEntity(typeof (Inherited1));

			Assert.That(() =>
			{
				inspector.AddAsTablePerClassHierarchyEntity(typeof (Inherited1));
				inspector.IsTablePerConcreteClass(typeof (Inherited1));
			}, Throws.TypeOf<MappingException>());
		}

		[Test]
		public void WhenRegisteredAsUnionSubclassThenCantRegisterAsJoinedSubclass()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof (MyClass));
			inspector.AddAsTablePerConcreteClassEntity(typeof (Inherited1));

			Assert.That(() =>
			{
				inspector.AddAsTablePerClassEntity(typeof (Inherited1));
				inspector.IsTablePerConcreteClass(typeof (Inherited1));
			}, Throws.TypeOf<MappingException>());
		}

		[Test]
		public void WhenRegisteredAsUnionSubclassThenIsEntity()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof(MyClass));
			inspector.AddAsTablePerConcreteClassEntity(typeof(Inherited1));

			Assert.That(inspector.IsEntity(typeof(Inherited1)), Is.True);
		}

		[Test]
		public void UnionSubclassIsAbstract()
		{
			//NH-3527
			var modelMapper = new ModelMapper();
			modelMapper.Class<MyClass>(c => { });
			modelMapper.UnionSubclass<Inherited1>(c =>
				{
					c.Abstract(true);
					c.Extends(typeof(MyClass));
				});

			var mappings = modelMapper.CompileMappingForAllExplicitlyAddedEntities();

			Assert.IsTrue(mappings.UnionSubclasses[0].@abstract);
			Assert.IsTrue(mappings.UnionSubclasses[0].extends == typeof(MyClass).FullName);
		}
	}
}