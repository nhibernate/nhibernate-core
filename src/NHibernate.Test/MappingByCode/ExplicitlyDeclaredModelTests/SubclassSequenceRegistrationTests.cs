using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;

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
			inspector.IsTablePerClassHierarchy(typeof(Inherited1)).Should().Be.True();
		}

		[Test]
		public void WhenRegisterSubclassWithNoRootThenThrows()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsTablePerClassHierarchyEntity(typeof(Inherited1));

			inspector.Executing(x => x.IsTablePerClassHierarchy(typeof(Inherited1))).Throws<MappingException>();
		}
	}
}