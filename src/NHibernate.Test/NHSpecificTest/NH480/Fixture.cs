using System;
using System.Globalization;
using System.Threading;
using NHibernate.Cfg;
using NHibernate.Properties;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH480
{
	/// <summary>
	/// This fixture is run in a Turkish locale because Turkish has special casing
	/// rules for I and i.
	/// </summary>
	/// <remarks>
	/// The rules are as follows:
	/// <list>
	///		<item>I (uppercase dotless i) is mapped to dotless lowercase i by ToLower</item>
	///		<item>Lowercase i with a dot is mapped to uppercase I with a dot above by ToUpper</item>
	/// </list>
	/// This test checks that field naming strategies handle this correctly.
	/// </remarks>
	[TestFixture]
	public class Fixture
	{
		private CultureInfo currentCulture = null;
		private CultureInfo currentUICulture = null;

		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			currentCulture = Thread.CurrentThread.CurrentCulture;
			currentUICulture = Thread.CurrentThread.CurrentUICulture;

			CultureInfo turkish = new CultureInfo("tr-TR");
			Thread.CurrentThread.CurrentCulture = turkish;
			Thread.CurrentThread.CurrentUICulture = turkish;
		}

		[TestFixtureTearDown]
		public void TestFixtureTearDown()
		{
			Thread.CurrentThread.CurrentCulture = currentCulture;
			Thread.CurrentThread.CurrentUICulture = currentUICulture;
		}

		[Test]
		public void CheckIII()
		{
			Assert.AreEqual("iii", new CamelCaseStrategy().GetFieldName("Iii"));
			Assert.AreEqual("_iii", new CamelCaseUnderscoreStrategy().GetFieldName("Iii"));

			Assert.AreEqual("iii", new LowerCaseStrategy().GetFieldName("III"));
			Assert.AreEqual("_iii", new LowerCaseUnderscoreStrategy().GetFieldName("III"));

			Assert.AreEqual("m_Iii", new PascalCaseMUnderscoreStrategy().GetFieldName("iii"));
			Assert.AreEqual("_Iii", new PascalCaseUnderscoreStrategy().GetFieldName("iii"));

			Assert.AreEqual("iii_iii_iii", ImprovedNamingStrategy.Instance.ColumnName("IiiIiiIii"));
		}
	}
}