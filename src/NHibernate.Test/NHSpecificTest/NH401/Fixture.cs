using System;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH401
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH401"; }
		}

		[Test]
		public void Merge()
		{
			object clubId;

			using (ISession s = OpenSession())
			{
				Club club1 = new Club();
				clubId = s.Save(club1);
				s.Flush();
			}

			Clubmember mem = new Clubmember();

			ISession sess2 = OpenSession();
			mem.Club = (Club) sess2.Get(typeof(Club), clubId);
			sess2.Close();

			ISession sess = OpenSession();
			mem.Expirydate = DateTime.Now.AddYears(1);
			mem.Joindate = DateTime.Now;

			sess.Merge(mem);
			sess.Flush();

			sess.Close();

			using (ISession s = OpenSession())
			{
				s.Delete("from System.Object");
				s.Flush();
			}
		}
	}
}