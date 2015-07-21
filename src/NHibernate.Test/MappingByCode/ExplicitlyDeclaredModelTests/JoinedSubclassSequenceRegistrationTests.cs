using NHibernate.Mapping.ByCode;
using NUnit.Framework;

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
			Assert.That(inspector.IsTablePerClass(typeof(Inherited1)), Is.True);
		}

		[Test]
		public void WhenRegisterJoinedSubclassWithNoRootThenThrows()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsTablePerClassEntity(typeof(Inherited1));

			Assert.That(() => inspector.IsTablePerClass(typeof(Inherited1)), Throws.TypeOf<MappingException>());
		}

		[Test]
		public void WhenRegisterJoinedSubclassWithNoRootThenCanAskForIsEntity()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsTablePerClassEntity(typeof(Inherited1));

			Assert.That(inspector.IsEntity(typeof(Inherited1)), Is.True);
		}
	}
}