using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using NHibernate.Cfg;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using NHibernate.Tool.hbm2ddl;
using NHibernate.DomainModel;
using System.IO;
using NUnit.Framework.SyntaxHelpers;
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
			Configuration cfg = new Configuration();
			cfg.AddResource("NHibernate.DomainModel.ParentChild.hbm.xml", assembly);

			BinaryFormatter formatter = new BinaryFormatter();
			var memoryStream = new MemoryStream();
			formatter.Serialize(memoryStream, cfg);
			memoryStream.Position = 0;
			cfg = formatter.Deserialize(memoryStream) as Configuration;
			SchemaExport export = new SchemaExport(cfg);
			export.Execute(true, true, false, true);
			var sf = cfg.BuildSessionFactory();
			using(var session=sf.OpenSession())
			using(var tran=session.BeginTransaction())
			{
				Parent parent = new Parent();
				Child child = new Child();
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

			using (var session = sf.OpenSession())
			using (var tran = session.BeginTransaction())
			{
				Parent parent = session.Get<Parent>(1L);
				Assert.That(parent.Count, Is.EqualTo(5));
				Assert.That(parent.X, Is.EqualTo(9));
				Assert.That(parent.Child, Is.Not.Null);
				Assert.That(parent.Child.X, Is.EqualTo(4));
				Assert.That(parent.Child.Count, Is.EqualTo(3));
				Assert.That(parent.Child.Parent, Is.EqualTo(parent));
			}

			
			using (var session = sf.OpenSession())
			using (var tran = session.BeginTransaction())
			{
				var p = session.Get<Parent>(1L);
				var c = session.Get<Child>(1L);
				session.Delete(c);
				session.Delete(p);
				tran.Commit();
			}
			using (var session = sf.OpenSession())
			using (var tran = session.BeginTransaction())
			{
				var p = session.Get<Parent>(1L);
				Assert.That(p, Is.Null);
			}
			export.Drop(true, true);
		}
	}
}
