namespace NHibernate.Validator.Tests.ResourceFixture
{
	using System.Globalization;
	using System.Reflection;
	using System.Resources;
	using NUnit.Framework;

	[TestFixture]
	public class ResourcesTest
	{
		[Test]
		public void CanGetResources()
		{
			ResourceManager rm = new ResourceManager("NHibernate.Validator.Resources.DefaultValidatorMessages",
			                                         Assembly.LoadFrom("NHibernate.Validator.dll"));

			string s_es = rm.GetString("validator.length",new CultureInfo("es"));
			string s_it = rm.GetString("validator.length", new CultureInfo("it"));

			Assert.AreNotEqual(s_es,s_it);
			
		}
	}
}