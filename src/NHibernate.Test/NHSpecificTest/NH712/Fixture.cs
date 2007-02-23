#if NET_2_0
using System;
using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH712
{
	[TestFixture]
	public class Fixture
	{
		[Test]
		public void Bug()
		{
			if (!Cfg.Environment.UseReflectionOptimizer)
			{
				Assert.Ignore("Test only works with reflection optimization enabled");
			}
			try
			{
				new Configuration()
					.AddResource(GetType().Namespace + ".Mappings.hbm.xml", GetType().Assembly)
					.BuildSessionFactory();
				Assert.Fail();
			}
			catch (MappingException ex)
			{
				Assert.IsTrue(ex.InnerException is MappingException);
			}
		}
	}
}
#endif