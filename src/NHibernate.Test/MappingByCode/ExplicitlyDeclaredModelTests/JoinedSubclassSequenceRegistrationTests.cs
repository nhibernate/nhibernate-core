using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode.ExplicitlyDeclaredModelTests
{
	public class JoinedSubclassSequenceRegistrationTests
	{
		private class MyClass
		{

		}
		private class Inherited1 : MyClass
		{

		}

		[Test]
		public void WhenRegisterJoinedSubclassBeforeRootThenIsRegistered()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsTablePerClassEntity(typeof(Inherited1));

			inspector.AddAsRootEntity(typeof(MyClass));
			inspector.IsTablePerClass(typeof(Inherited1)).Should().Be.True();
		}

		[Test]
		public void WhenRegisterJoinedSubclassWithNoRootThenThrows()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsTablePerClassEntity(typeof(Inherited1));

			inspector.Executing(x => x.IsTablePerClass(typeof(Inherited1))).Throws<MappingException>();
		}

		[Test]
		public void WhenRegisterJoinedSubclassWithNoRootThenCanAskForIsEntity()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsTablePerClassEntity(typeof(Inherited1));

			inspector.IsEntity(typeof(Inherited1)).Should().Be.True();
		}
	}
}