using System;
using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.Id;
using NUnit.Framework;

namespace NHibernate.Test.IdGen.NativeGuid
{
	[TestFixture]
	public class NativeGuidGeneratorFixture
	{
		protected Configuration cfg;
		protected ISessionFactoryImplementor sessions;

		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			cfg = new Configuration();
			if (TestConfigurationHelper.hibernateConfigFile != null)
				cfg.Configure(TestConfigurationHelper.hibernateConfigFile);

			sessions = (ISessionFactoryImplementor) cfg.BuildSessionFactory();
		}

		[Test]
		public void ReturnedValueIsGuid()
		{
			try
			{
				var str = Dialect.Dialect.GetDialect().SelectGUIDString;	
			}
			catch (NotSupportedException)
			{
				Assert.Ignore("This test does not apply to {0}", Dialect.Dialect.GetDialect());
			}
			 
			var gen = new NativeGuidGenerator();
			using (ISession s = sessions.OpenSession())
			{
				object result = gen.Generate((ISessionImplementor)s, null);
				Assert.That(result, Is.TypeOf(typeof (Guid)));
				Assert.That(result, Is.Not.EqualTo(Guid.Empty));
			}
		}
	}
}