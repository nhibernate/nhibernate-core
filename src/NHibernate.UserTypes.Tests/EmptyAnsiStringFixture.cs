using System;
using System.Collections;

using NUnit.Framework;

namespace NHibernate.UserTypes.Tests
{
	[TestFixture]
	public class EmptyAnsiStringFixture : TestCase
	{
		protected override IList Mappings
		{
			get { return new string[] {"EmptyAnsiStringClass.hbm.xml"}; }
		}

		[Test]
		public void NullValueInProperty()
		{
			EmptyStringClass emptyString1 = new EmptyStringClass();
			emptyString1.Id = 1;
			emptyString1.NotNullString = null;

			ISession s = sessions.OpenSession();
			s.Save(emptyString1);
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			emptyString1 = s.Load(typeof(EmptyStringClass), 1) as EmptyStringClass;
			Assert.AreEqual(String.Empty, emptyString1.NotNullString, "should not have created a null string");
			s.Delete(emptyString1);
			s.Flush();
			s.Close();
		}

		[Test]
		public void EmptyValueInProperty()
		{
			string testValue = "a test value";

			EmptyStringClass emptyString1 = new EmptyStringClass();
			emptyString1.Id = 1;
			emptyString1.NotNullString = String.Empty;

			ISession s = sessions.OpenSession();
			s.Save(emptyString1);
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			emptyString1 = s.Load(typeof(EmptyStringClass), 1) as EmptyStringClass;
			emptyString1.NotNullString = testValue;
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			emptyString1 = s.Load(typeof(EmptyStringClass), 1) as EmptyStringClass;
			Assert.AreEqual(testValue, emptyString1.NotNullString, "EmptyAnsiStringType did not read/write value correctly");
			emptyString1.NotNullString = String.Empty;
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			emptyString1 = s.Load(typeof(EmptyStringClass), 1) as EmptyStringClass;
			Assert.AreEqual(String.Empty, emptyString1.NotNullString, "even though null in db, should be an empty string");
			s.Delete(emptyString1);
			s.Flush();
			s.Close();
		}
	}
}