using System;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH552
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH552"; }
		}

		[Test]
		public void DeleteAndResave()
		{
			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				Question q = new Question();
				q.Id = 1;
				session.Save(q);
				session.Delete(q);
				session.Save(q);

				Answer a = new Answer();
				a.Id = 1;
				a.Question = q;
				session.Save(a);
				tx.Commit();
			}

			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				session.Delete("from Answer");
				session.Delete("from Question");
				tx.Commit();
			}
		}
	}
}