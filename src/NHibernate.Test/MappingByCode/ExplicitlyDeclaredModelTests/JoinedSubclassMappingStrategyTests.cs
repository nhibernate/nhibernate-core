using NHibernate.Mapping.ByCode;
using NUnit.Framework;

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

			Assert.That(inspector.IsTablePerClass(typeof(Inherited1)), Is.True);
			Assert.That(inspector.IsTablePerClassHierarchy(typeof(Inherited1)), Is.False);
			Assert.That(inspector.IsTablePerConcreteClass(typeof(Inherited1)), Is.False);
		}

		[Test]
		public void WhenRegisteredAsDeppJoinedSubclassThenIsRegistered()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof(MyClass));
			inspector.AddAsTablePerClassEntity(typeof(Inherited2));

			Assert.That(inspector.IsTablePerClass(typeof(Inherited2)), Is.True);
			Assert.That(inspector.IsTablePerClassHierarchy(typeof(Inherited2)), Is.False);
			Assert.That(inspector.IsTablePerConcreteClass(typeof(Inherited2)), Is.False);
		}

		[Test]
		public void WhenRegisteredAsJoinedSubclassThenCantRegisterAsSubclass()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof (MyClass));
			inspector.AddAsTablePerClassEntity(typeof (Inherited1));

			Assert.That(() =>
			{
				inspector.AddAsTablePerClassHierarchyEntity(typeof (Inherited1));
				inspector.IsTablePerClass(typeof (Inherited1));
			}, Throws.TypeOf<MappingException>());
		}

		[Test]
		public void WhenRegisteredAsJoinedSubclassThenCantRegisterAsUnionSubclass()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof (MyClass));
			inspector.AddAsTablePerClassEntity(typeof (Inherited1));

			Assert.That(() =>
			{
				inspector.AddAsTablePerConcreteClassEntity(typeof (Inherited1));
				inspector.IsTablePerClass(typeof (Inherited1));
			}, Throws.TypeOf<MappingException>());
		}

		[Test]
		public void WhenRegisteredAsJoinedSubclassThenIsEntity()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsRootEntity(typeof(MyClass));
			inspector.AddAsTablePerClassEntity(typeof(Inherited1));

			Assert.That(inspector.IsEntity(typeof(Inherited1)), Is.True);
		}

		[Test]
		public void JoinedSubclassIsAbstract()
		{
			//NH-3527
			var modelMapper = new ModelMapper();
			modelMapper.Class<MyClass>(c => { });
			modelMapper.JoinedSubclass<Inherited1>(c =>
			{
				c.Abstract(true);
				c.Extends(typeof(MyClass));
			});

			var mappings = modelMapper.CompileMappingForAllExplicitlyAddedEntities();

			Assert.IsTrue(mappings.JoinedSubclasses[0].@abstract);
			Assert.IsTrue(mappings.JoinedSubclasses[0].extends == typeof(MyClass).FullName);
		}
	}
}