using System.Reflection;
using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.Persister.Entity;
using NUnit.Framework;
using SharpTestsEx;

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
			persister.IsBatchLoadable.Should().Be.False();

			// hack
			fieldInfo.GetValue(persister).Should().Be(1);
		}

		[Test]
		public void WhenDefaultAndNoSpecificThenUseDefault()
		{
			var cfg = TestConfigurationHelper.GetDefaultConfiguration();
			cfg.SetProperty(Environment.DefaultBatchFetchSize, "20");

			cfg.AddResource("NHibernate.Test.NHSpecificTest.NH2147.Mappings.hbm.xml", GetType().Assembly);
			var sf = (ISessionFactoryImplementor)cfg.BuildSessionFactory();
			var persister = sf.GetEntityPersister("MyClassWithoutBatchSize");

			persister.IsBatchLoadable.Should().Be.True();

			// hack
			fieldInfo.GetValue(persister).Should().Be(20);
		}

		[Test]
		public void WhenDefaultAndSpecificThenUseSpecific()
		{
			var cfg = TestConfigurationHelper.GetDefaultConfiguration();
			cfg.SetProperty(Environment.DefaultBatchFetchSize, "20");

			cfg.AddResource("NHibernate.Test.NHSpecificTest.NH2147.Mappings.hbm.xml", GetType().Assembly);
			var sf = (ISessionFactoryImplementor)cfg.BuildSessionFactory();
			var persister = sf.GetEntityPersister("MyClassWithBatchSize");

			persister.IsBatchLoadable.Should().Be.True();

			// hack
			fieldInfo.GetValue(persister).Should().Be(10);
		}
	}
}