using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.ExplicitlyDeclaredModelTests
{
	public class SubclassSequenceRegistrationTests
	{
		private class MyClass
		{

		}
		private class Inherited1 : MyClass
		{

		}

		[Test]
		public void WhenRegisterSubclassBeforeRootThenIsRegistered()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsTablePerClassHierarchyEntity(typeof(Inherited1));

			inspector.AddAsRootEntity(typeof(MyClass));
			Assert.That(inspector.IsTablePerClassHierarchy(typeof(Inherited1)), Is.True);
		}

		[Test]
		public void WhenRegisterSubclassWithNoRootThenThrows()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsTablePerClassHierarchyEntity(typeof(Inherited1));

			Assert.That(() => inspector.IsTablePerClassHierarchy(typeof(Inherited1)), Throws.TypeOf<MappingException>());
		}
	}
}