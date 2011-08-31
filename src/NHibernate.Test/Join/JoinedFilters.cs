using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NHibernate.Test.Join
{
	[TestFixture]
	public class JoinedFilters : TestCase
	{
		protected override IList Mappings
		{
			get { return new [] {"Join.TennisPlayer.hbm.xml"}; }
		}

		protected override void OnTearDown()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Delete("from TennisPlayer");
				tx.Commit();
			}
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		[Test]
		public void FilterOnPrimaryTable()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.EnableFilter("NameFilter").SetParameter("name", "Nadal");

				CreateAndSavePlayer(s, "Nadal", "Babolat");
				CreateAndSavePlayer(s, "Rodick", "Wilson");
				s.Flush();

				IList<TennisPlayer> people = s.CreateCriteria<TennisPlayer>().List<TennisPlayer>();
				Assert.AreEqual(1, people.Count);
				Assert.AreEqual("Nadal", people[0].Name);

				tx.Commit();
			}
		}

		[Test]
		public void FilterOnJoinedTable()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.EnableFilter("RacquetFilter").SetParameter("racquet", "Babolat");

				CreateAndSavePlayer(s, "Nadal", "Babolat");
				CreateAndSavePlayer(s, "Rodick", "Wilson");
				s.Flush();

				IList<TennisPlayer> people = s.CreateCriteria<TennisPlayer>().List<TennisPlayer>();
				Assert.AreEqual(1, people.Count);
				Assert.AreEqual("Babolat", people[0].Racquet);

				tx.Commit();
			}
		}

		private static void CreateAndSavePlayer(ISession session, string name, string make)
		{
			var s = new TennisPlayer() {Name = name, Racquet = make};
			session.Save(s);
		}
	}
}
