using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH2673
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		private readonly BinaryFormatter _formatter;

		public Fixture()
		{
			_formatter = new BinaryFormatter
			{
#if !NETFX
				SurrogateSelector = new SerializationHelper.SurrogateSelector()
#endif
			};
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var role1 = new Role
				{
					Name = "Role1",
					Key = 11
				};
				var role2 = new Role
				{
					Name = "Role2",
					Key = 22
				};
				var resource1 = new Resource
				{
					Key = 1,
					Name = "Resource1",
					ResourceRole = role1
				};
				var resource2 = new Resource
				{
					Key = 2,
					Name = "Resource2",
					ResourceRole = role2,
					Manager = resource1
				};
				session.Save(role1);
				session.Save(role2);
				session.Save(resource1);
				session.Save(resource2);

				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.CreateQuery("delete from Resource").ExecuteUpdate();
				session.CreateQuery("delete from Role").ExecuteUpdate();
				session.CreateQuery("delete from System.Object").ExecuteUpdate();

				transaction.Commit();
			}
		}

		[Test]
		public void TestSerialization()
		{
			using (var session = OpenSession())
			using (var t = session.BeginTransaction())
			{
				var resource = session.Get<Resource>(2);
				session.Get<Resource>(1);

				var serialized = Serialize(resource);
				Deserialize(serialized);

				t.Commit();
			}
		}

		[Test]
		public void TestSerialization2()
		{
			using (var session = OpenSession())
			using (var t = session.BeginTransaction())
			{
				session.Get<Resource>(1);
				var resource = session.Get<Resource>(2);

				var serialized = Serialize(resource);
				Deserialize(serialized);

				t.Commit();
			}
		}
		private byte[] Serialize(object value)
		{
			using (var stream = new MemoryStream())
			{
				_formatter.Serialize(stream, value);
				var result = new byte[stream.Length];
				stream.Position = 0;
				stream.Read(result, 0, (int) stream.Length);
				stream.Close();
				return result;
			}
		}

		private object Deserialize(byte[] state)
		{
			using (var stream = new MemoryStream())
			{
				stream.Write(state, 0, state.Length);
				stream.Position = 0;
				return _formatter.Deserialize(stream);
			}
		}
	}
}
