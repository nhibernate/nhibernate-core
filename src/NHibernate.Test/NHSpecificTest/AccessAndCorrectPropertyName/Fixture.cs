using System.Reflection;
using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.AccessAndCorrectPropertyName
{
	[TestFixture]
	public class Fixture
	{
		private const string ns = "NHibernate.Test.NHSpecificTest.AccessAndCorrectPropertyName.";

		[Test]
		public void WrongPropertyNameForCamelcaseUnderscoreShouldThrow()
		{
			//default-access="field.camelcase-underscore" on entity
			var cfg = new Configuration();
			Assert.Throws<MappingException>(() =>
				cfg.AddResource(ns + "PersonMapping.hbm.xml", Assembly.GetExecutingAssembly()));
		}

		[Test]
		public void WrongPropertyNameForCamelcaseShouldThrow()
		{
			//default-access="field.camelcase" on property
			var cfg = new Configuration();
			Assert.Throws<MappingException>(() =>
				cfg.AddResource(ns + "DogMapping.hbm.xml", Assembly.GetExecutingAssembly()));
		}
	}
}
