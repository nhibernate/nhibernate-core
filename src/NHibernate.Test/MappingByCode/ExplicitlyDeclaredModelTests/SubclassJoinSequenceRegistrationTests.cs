using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode.ExplicitlyDeclaredModelTests
{
	public class SubclassJoinSequenceRegistrationTests
	{
		private class MyClass
		{

		}
		private class Inherited1 : MyClass
		{

		}

		[Test]
		public void WhenRegisterSubclassJoinBeforeRootThenIsRegistered()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsTablePerClassHierarchyJoinEntity(typeof(Inherited1));

			inspector.AddAsRootEntity(typeof(MyClass));
			inspector.IsTablePerClassHierarchyJoin(typeof(Inherited1)).Should().Be.True();
		}

		[Test]
		public void WhenRegisterSubclassWithNoRootThenThrows()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsTablePerClassHierarchyJoinEntity(typeof(Inherited1));

			inspector.Executing(x => x.IsTablePerClassHierarchyJoin(typeof(Inherited1))).Throws<MappingException>();
		}
	}
}