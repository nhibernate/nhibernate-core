using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2880
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		private Guid _id;

		protected override void OnSetUp()
		{
			using (ISession s = Sfi.OpenSession())
			{
				using (ITransaction t = s.BeginTransaction())
				{
					Entity1 e1 = new Entity1();
					Entity2 e2 = new Entity2();
					e1.Entity2 = e2;
					e2.Text = "Text";

					s.Save(e1);
					s.Save(e2);

					_id = e1.Id;

					t.Commit();
				}
			}
		}

		[Test]
		public void ProxiesFromDeserializedSessionsCanBeLoaded()
		{
			MemoryStream sessionMemoryStream;

			using (ISession s = Sfi.OpenSession())
			{
				using (ITransaction t = s.BeginTransaction())
				{
					Entity1 e = s.Get<Entity1>(_id);
					t.Commit();
				}

				sessionMemoryStream = new MemoryStream();
				BinaryFormatter writer = new BinaryFormatter();
				writer.Serialize(sessionMemoryStream, s);
			}

			sessionMemoryStream.Seek(0, SeekOrigin.Begin);
			BinaryFormatter reader = new BinaryFormatter();
			ISession restoredSession = (ISession)reader.Deserialize(sessionMemoryStream);

			Entity1 e1 = restoredSession.Get<Entity1>(_id);
			Entity2 e2 = e1.Entity2;
			Assert.IsNotNull(e2);
			Assert.AreEqual("Text", e2.Text);

			restoredSession.Dispose();
		}

		[Test]
		public void EnabledFiltersStillHaveFilterDefinitionOnDeserializedSessions()
		{
			MemoryStream sessionMemoryStream;

			using (ISession s = Sfi.OpenSession())
			{
				s.EnableFilter("myFilter");

				sessionMemoryStream = new MemoryStream();
				BinaryFormatter writer = new BinaryFormatter();
				writer.Serialize(sessionMemoryStream, s);
			}

			sessionMemoryStream.Seek(0, SeekOrigin.Begin);
			BinaryFormatter reader = new BinaryFormatter();
			ISession restoredSession = (ISession)reader.Deserialize(sessionMemoryStream);

			Assert.IsNotNull(restoredSession.GetEnabledFilter("myFilter").FilterDefinition);

			restoredSession.Dispose();
		}

		protected override void OnTearDown()
		{
			using (ISession s = Sfi.OpenSession())
			{
				using (ITransaction t = s.BeginTransaction())
				{
					s.Delete("from Entity1");
					s.Delete("from Entity2");
					t.Commit();
				}
			}
		}
	}
}