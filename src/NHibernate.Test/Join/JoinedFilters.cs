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
			get 
			{ 
				return new []
				{
					"Join.TennisPlayer.hbm.xml",
					"Join.Person.hbm.xml"
				}; 
			}
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

				CreatePlayers(s);

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
				s.EnableFilter("MakeFilter").SetParameter("make", "Babolat");

				CreatePlayers(s);

				IList<TennisPlayer> people = s.CreateCriteria<TennisPlayer>().List<TennisPlayer>();
				Assert.AreEqual(1, people.Count);
				Assert.AreEqual("Babolat", people[0].RacquetMake);

				tx.Commit();
			}
		}

		[Test]
		public void FilterOnJoinedTableWithRepeatedColumn()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.EnableFilter("ModelFilter").SetParameter("model", "AeroPro Drive");

				CreatePlayers(s);

				IList<TennisPlayer> people = s.CreateCriteria<TennisPlayer>().List<TennisPlayer>();
				Assert.AreEqual(1, people.Count);
				Assert.AreEqual("AeroPro Drive", people[0].RacquetModel);

				tx.Commit();
			}
		}

		private static void CreatePlayers(ISession s)
		{
			CreateAndSavePlayer(s, "Nadal", "Babolat", "AeroPro Drive");
			CreateAndSavePlayer(s, "Federer", "Wilson", "Six.One Tour BLX");
			s.Flush();
		}

		private static void CreateAndSavePlayer(ISession session, string name, string make, string model)
		{
			var s = new TennisPlayer() {Name = name, RacquetMake = make, RacquetModel = model};
			session.Save(s);
		}
	}
}
