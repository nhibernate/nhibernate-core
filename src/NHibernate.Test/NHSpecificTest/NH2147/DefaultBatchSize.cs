using System.Reflection;
using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.Persister.Entity;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2147
{
	public class DefaultBatchSize
	{
		private const BindingFlags DefaultFlags =
			BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

		private readonly FieldInfo fieldInfo = typeof(AbstractEntityPersister).GetField("batchSize", DefaultFlags);

		[Test]
		public void WhenNoDefaultAndNoSpecificThenUse1()
		{
			var cfg = TestConfigurationHelper.GetDefaultConfiguration();
			cfg.AddResource("NHibernate.Test.NHSpecificTest.NH2147.Mappings.hbm.xml", GetType().Assembly);
			var sf = (ISessionFactoryImplementor)cfg.BuildSessionFactory();
			var persister = sf.GetEntityPersister("MyClassWithoutBatchSize");
			Assert.That(persister.IsBatchLoadable, Is.False);

			// hack
			Assert.That(fieldInfo.GetValue(persister), Is.EqualTo(1));
		}

		[Test]
		public void WhenDefaultAndNoSpecificThenUseDefault()
		{
			var cfg = TestConfigurationHelper.GetDefaultConfiguration();
			cfg.SetProperty(Environment.DefaultBatchFetchSize, "20");

			cfg.AddResource("NHibernate.Test.NHSpecificTest.NH2147.Mappings.hbm.xml", GetType().Assembly);
			var sf = (ISessionFactoryImplementor)cfg.BuildSessionFactory();
			var persister = sf.GetEntityPersister("MyClassWithoutBatchSize");

			Assert.That(persister.IsBatchLoadable, Is.True);

			// hack
			Assert.That(fieldInfo.GetValue(persister), Is.EqualTo(20));
		}

		[Test]
		public void WhenDefaultAndSpecificThenUseSpecific()
		{
			var cfg = TestConfigurationHelper.GetDefaultConfiguration();
			cfg.SetProperty(Environment.DefaultBatchFetchSize, "20");

			cfg.AddResource("NHibernate.Test.NHSpecificTest.NH2147.Mappings.hbm.xml", GetType().Assembly);
			var sf = (ISessionFactoryImplementor)cfg.BuildSessionFactory();
			var persister = sf.GetEntityPersister("MyClassWithBatchSize");

			Assert.That(persister.IsBatchLoadable, Is.True);

			// hack
			Assert.That(fieldInfo.GetValue(persister), Is.EqualTo(10));
		}
	}
}