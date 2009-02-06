using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using NHibernate.Engine;
using NHibernate.Impl;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace NHibernate.Test.SessionFactoryTest
{
	[TestFixture,Ignore]
	public class SessionFactorySerializationFixture:TestCase
	{
		protected ISessionFactory SerializeAndDeserialize()
		{
			var ncfg = base.cfg;
			ncfg.Properties["session_factory_name"] = "myfactory";
			var sessionFactory = ncfg.BuildSessionFactory();
			var uuidField = typeof (SessionFactoryImpl).GetField("uuid",
			                                                     BindingFlags.Instance | 
																 BindingFlags.NonPublic |
			                                                     BindingFlags.GetField);
			var uuid = uuidField.GetValue(sessionFactory) as string;
			var stream = new MemoryStream();
			var formatter = new BinaryFormatter();
			formatter.Serialize(stream, sessionFactory);
			sessionFactory.Dispose();
			stream.Position = 0;
			var factory = formatter.Deserialize(stream) as ISessionFactoryImplementor;
			stream.Dispose();
			return factory;
		}
		protected void Serialize(ISessionFactoryImplementor factory,MemoryStream ms)
		{
			var formatter = new BinaryFormatter();
			formatter.Serialize(ms, this.sessions);
		}
		[Test]
		public void SessionFactory_should_be_serializable()
		{
			using (var stream = new MemoryStream())
			{
				Serialize(this.sessions, stream);
				Assert.That(stream.Length, Is.Not.EqualTo(0));
			}
		}

		[Test]
		public void SessionFactory_should_be_deserialized()
		{
			var factory = SerializeAndDeserialize();
			Assert.That(factory, Is.Not.Null);
		}
		[Test]
		public void CRUD_works_correctly_after_deserialization()
		{
			var sessionFactory = SerializeAndDeserialize();

			using(var session=sessionFactory.OpenSession())
			using(var tran=session.BeginTransaction())
			{
				var obj = new Item {Name = "object1"};
				var obj2 = new Item {Name = "object2"};
				obj.Children.Add(obj2);
				obj2.Parent = obj;
				session.Save(obj);
				session.Save(obj);
				tran.Commit();
			}

			using (var session = sessionFactory.OpenSession())
			using (var tran = session.BeginTransaction())
			{
				var item = session.Get<Item>(1);
				Assert.That(item, Is.Not.Null);
				Assert.That(item.Children.Count, Is.EqualTo(1));
				
				item.Name = "object5";
				session.Save(item);
				tran.Commit();
			}

			using(var session=sessionFactory.OpenSession())
			{
				var item = session.Get<Item>(1);
				Assert.That(item.Name, Is.EqualTo("object5"));
			}

			using (var session = sessionFactory.OpenSession())
			{
				var item = session.Get<Item>(1);
				session.Delete(item);
				session.Evict(item);
				item = session.Get<Item>(1);
				Assert.That(item, Is.Null);
			}
			
		}
		protected override System.Collections.IList Mappings
		{
			get
			{
				return new string[]
				       	{
				       		"SessionFactoryTest.Item.hbm.xml",
							
				       	};
			}
		}
		protected override string MappingsAssembly
		{
			get
			{
				return "NHibernate.Test";
			}
		}
	}
}
