using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH2619Proxy
{
	public class Fixture : BugTestCase
	{
		protected override void Configure(Configuration configuration)
		{
			base.Configure(configuration);
			configuration.SetProperty("use_proxy_validator", "false");
		}

		[Test]
		public void TestOk()
		{
			using (ISession s = OpenSession())
			{
				using (s.BeginTransaction()) {
					Assert.DoesNotThrow(() => s.Load<ClassWithGenericNonVirtualMethod>(1));
				}
			}
		}
	}
}
