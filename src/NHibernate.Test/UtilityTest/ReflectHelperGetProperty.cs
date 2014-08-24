using NHibernate.Util;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.UtilityTest
{
	public class ReflectHelperGetProperty
	{
		private class Entity
		{
			public int Id { get; set; }
		}
		private interface IHasSomething
		{
			string Something { get; }
		}
		private class Person : Entity, IHasSomething
		{
			private string _firstName;

			public string FiRsTNaMe
			{
				get { return _firstName; }
				set { _firstName = value; }
			}

			#region IHasSomething Members

			string IHasSomething.Something
			{
				get { throw new System.NotImplementedException(); }
			}

			#endregion

			public string Normal { get; set; }
		}

		[Test]
		public void WhenNullSourceThenNotFound()
		{
			System.Type source= null;
			source.HasProperty("whatever").Should().Be.False();
		}

		[Test]
		public void WhenNullNameThenNotFound()
		{
			typeof(Person).HasProperty(null).Should().Be.False();
		}

		[Test]
		public void WhenPropertyIsInClassThenFound()
		{
			typeof(Person).HasProperty("Normal").Should().Be.True();
		}

		[Test]
		public void WhenPropertyIsInBaseClassThenFound()
		{
			typeof(Person).HasProperty("Id").Should().Be.True();
		}

		[Test]
		public void WhenPropertyIsExplicitImplementationOfInterfaceThenFound()
		{
			typeof(Person).HasProperty("Something").Should().Be.True();
		}

		[Test]
		public void WhenFieldThenNotFound()
		{
			typeof(Person).HasProperty("_firstName").Should().Be.False();
		}

		[Test]
		public void WhenPropertyNameWithDifferentCaseThenNotFound()
		{
			typeof(Person).HasProperty("FirstName").Should().Be.False();
		}
	}
}