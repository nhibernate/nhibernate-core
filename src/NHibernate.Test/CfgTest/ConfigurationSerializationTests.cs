using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using NHibernate.Cfg;
using NHibernate.DomainModel;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace NHibernate.Test.CfgTest
{
	[TestFixture]
	public class ConfigurationSerializationTests
	{
		[Test]
		public void Configuration_should_be_serializable()
		{
			NHAssert.HaveSerializableAttribute(typeof (Configuration));
		}

		[Test]
		public void Basic_CRUD_should_work()
		{
			Assembly assembly = Assembly.Load("NHibernate.DomainModel");
			var cfg = new Configuration();
			if (TestConfigurationHelper.hibernateConfigFile != null)
			{
				cfg.Configure(TestConfigurationHelper.hibernateConfigFile);
			}
			cfg.AddResource("NHibernate.DomainModel.ParentChild.hbm.xml", assembly);

			var formatter = new BinaryFormatter();
			var memoryStream = new MemoryStream();
			formatter.Serialize(memoryStream, cfg);
			memoryStream.Position = 0;
			cfg = formatter.Deserialize(memoryStream) as Configuration;
			Assert.That(cfg, Is.Not.Null);

			var export = new SchemaExport(cfg);
			export.Execute(true, true, false);
			ISessionFactory sf = cfg.BuildSessionFactory();
			using (ISession session = sf.OpenSession())
			{
				using (ITransaction tran = session.BeginTransaction())
				{
					var parent = new Parent();
					var child = new Child();
					parent.Child = child;
					parent.X = 9;
					parent.Count = 5;
					child.Parent = parent;
					child.Count = 3;
					child.X = 4;
					session.Save(parent);
					session.Save(child);
					tran.Commit();
				}
			}

			using (ISession session = sf.OpenSession())
			{
				var parent = session.Get<Parent>(1L);
				Assert.That(parent.Count, Is.EqualTo(5));
				Assert.That(parent.X, Is.EqualTo(9));
				Assert.That(parent.Child, Is.Not.Null);
				Assert.That(parent.Child.X, Is.EqualTo(4));
				Assert.That(parent.Child.Count, Is.EqualTo(3));
				Assert.That(parent.Child.Parent, Is.EqualTo(parent));
			}

			using (ISession session = sf.OpenSession())
			{
				using (ITransaction tran = session.BeginTransaction())
				{
					var p = session.Get<Parent>(1L);
					var c = session.Get<Child>(1L);
					session.Delete(c);
					session.Delete(p);
					tran.Commit();
				}
			}
			using (ISession session = sf.OpenSession())
			{
				var p = session.Get<Parent>(1L);
				Assert.That(p, Is.Null);
			}
			export.Drop(true, true);
		}
	}
}