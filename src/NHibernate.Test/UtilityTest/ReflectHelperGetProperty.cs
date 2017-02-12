using NHibernate.Util;
using NUnit.Framework;

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
			Assert.That(source.HasProperty("whatever"), Is.False);
		}

		[Test]
		public void WhenNullNameThenNotFound()
		{
			Assert.That(typeof(Person).HasProperty(null), Is.False);
		}

		[Test]
		public void WhenPropertyIsInClassThenFound()
		{
			Assert.That(typeof(Person).HasProperty("Normal"), Is.True);
		}

		[Test]
		public void WhenPropertyIsInBaseClassThenFound()
		{
			Assert.That(typeof(Person).HasProperty("Id"), Is.True);
		}

		[Test]
		public void WhenPropertyIsExplicitImplementationOfInterfaceThenFound()
		{
			Assert.That(typeof(Person).HasProperty("Something"), Is.True);
		}

		[Test]
		public void WhenFieldThenNotFound()
		{
			Assert.That(typeof(Person).HasProperty("_firstName"), Is.False);
		}

		[Test]
		public void WhenPropertyNameWithDifferentCaseThenNotFound()
		{
			Assert.That(typeof(Person).HasProperty("FirstName"), Is.False);
		}
	}
}