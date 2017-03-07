using System;
using System.Linq;
using NHibernate.Driver;
using NHibernate.Engine;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3252
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override bool AppliesTo(ISessionFactoryImplementor factory)
		{
			return factory.ConnectionProvider.Driver is SqlClientDriver;
		}

		[Test]
		public void VerifyThatWeCanSaveAndLoad()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{

				session.Save(new Note { Text = new String('0', 9000) });
				transaction.Commit();
			}

			using (var session = OpenSession())
			using (session.BeginTransaction())
			{

				var note = session.Query<Note>().First();
				Assert.AreEqual(9000, note.Text.Length);
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");

				session.Flush();
				transaction.Commit();
			}
		}
	}
}
