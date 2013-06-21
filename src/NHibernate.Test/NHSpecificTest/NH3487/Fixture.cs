using System;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using NHibernate.Linq;
using NHibernate.Collection;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3487
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		private Key key1;
		private Key key2;

		public override string BugNumber
		{
			get { return "NH3487"; }
		}

		protected override void OnSetUp()
		{
			using (ISession session = OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					key1 = new Key() { Id = 1 };
					var entity1 = new Entity { Id = key1, Name = "Bob1" };
					session.Save(entity1);

					key2 = new Key() { Id = 2 };
					var entity2 = new Entity { Id = key2, Name = "Bob2" };
					session.Save(entity2);

					session.Flush();
					transaction.Commit();
				}
			}
		}

		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					session.Delete("from System.Object");

					session.Flush();
					transaction.Commit();
				}
			}
		}

		[Test]
		public void Test()
		{
			IFormatter formatter = new BinaryFormatter();
			byte[] serializedSessionArray;

			using (ISession session = OpenSession())
			{
				using (session.BeginTransaction())
				{
					var entity1 = session.Get<Entity>(key1);
					var entity2 = session.Get<Entity>(key2);
				}

				session.Disconnect();
				using (var serializationStream = new MemoryStream())
				{
					formatter.Serialize(serializationStream, session);
					serializationStream.Close();
					serializedSessionArray = serializationStream.ToArray();
				}
			}

			ISession deserializedSession;
			using (var serializationStream = new MemoryStream(serializedSessionArray))
			{
				deserializedSession = (ISession)formatter.Deserialize(serializationStream);
			}

		}
	}
}