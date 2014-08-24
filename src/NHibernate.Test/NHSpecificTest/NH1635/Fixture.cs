using System.Text;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1635
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		private void CreateTestContext()
		{
			var t1 = new ForumThread {Id = 1, Name = "Thread 1"};
			var t2 = new ForumThread {Id = 2, Name = "Thread 2"};
			var m1 = new ForumMessage {Id = 1, Name = "Thread 1: Message 1", ForumThread = t1};
			var m2 = new ForumMessage {Id = 2, Name = "Thread 1: Message 2", ForumThread = t1};
			var m3 = new ForumMessage {Id = 3, Name = "Thread 2: Message 1", ForumThread = t2};

			t1.Messages.Add(m1);
			t1.Messages.Add(m2);
			t2.Messages.Add(m3);

			using (ISession session = OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					session.Save(t1);
					session.Save(t2);

					transaction.Commit();
				}
			}
		}

		private void CleanUp()
		{
			using (ISession session = OpenSession())
			{
				session.Delete("from ForumMessage");
				session.Delete("from ForumThread");
				session.Flush();
			}
		}

		protected override void CreateSchema()
		{
			var script = new StringBuilder();
			new SchemaExport(cfg).Create(sl=> script.Append(sl) , true);
			Assert.That(script.ToString(), Is.Not.StringContaining("LatestMessage"));
		}

		[Test]
		public void Test()
		{
			CreateTestContext();
			using (ISession session = OpenSession())
			{
				var thread = session.Get<ForumThread>(1);

				Assert.IsNotNull(thread.LatestMessage);
				Assert.IsTrue(thread.LatestMessage.Id == 2);

				session.Flush();
			}
			CleanUp();
		}
	}
}