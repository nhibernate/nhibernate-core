namespace NHibernate.Validator.Tests.Resources
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

			string s = rm.GetString("validator.length");
			string s2 = rm.GetString("validator.length",new CultureInfo("es"));
		}
	}
}