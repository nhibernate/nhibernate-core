using System;
using System.Collections;
using Iesi.Collections;
using NHibernate.DomainModel.NHSpecific;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest
{
	/// <summary>
	/// Summary description for MapTest.
	/// </summary>
	[TestFixture]
	public class MapFixture : TestCase
	{
		private DateTime testDateTime = new DateTime(2003, 8, 16);
		private DateTime updateDateTime = new DateTime(2003, 8, 17);

		protected override IList Mappings
		{
			get
			{
				return new string[]
					{
						"NHSpecific.Parent.hbm.xml",
						"NHSpecific.Child.hbm.xml",
						"NHSpecific.SexType.hbm.xml",
						"NHSpecific.Team.hbm.xml"
					};
			}
		}

		protected override void OnTearDown()
		{
			using (ISession session = sessions.OpenSession())
			{
				session.Delete("from Team");
				session.Delete("from Child");
				session.Delete("from Parent");
				session.Delete("from SexType");
				session.Flush();
			}
		}


		[Test]
		public void TestSelect()
		{
			TestInsert();

			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			ICriteria chiefsCriteria = s.CreateCriteria(typeof(Team));
			chiefsCriteria.Add(Expressions.Expression.Eq("Name", "Chiefs"));

			Team chiefs = (Team) chiefsCriteria.List()[0];
			IList players = chiefs.Players;

			Parent parentDad = (Parent) s.Load(typeof(Parent), 1);
			Child amyJones = (Child) s.Load(typeof(Child), 2);
			Child[] friends = amyJones.Friends;

			Child childOneRef = amyJones.FirstSibling;

			t.Commit();
			s.Close();
		}

		[Test]
		public void TestSort()
		{
			TestInsert();

			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();


			Parent bobJones = (Parent) s.Load(typeof(Parent), 1);
			ISet friends = bobJones.AdultFriends;

			int currentId = 0;
			int previousId = 0;

			foreach (Parent friend in friends)
			{
				previousId = currentId;
				currentId = friend.Id;

				Assert.IsTrue(currentId > previousId, "Current should have a higher Id than previous");
			}

			t.Commit();
			s.Close();
		}

		[Test]
		public void TestInsert()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			SexType male = new SexType();
			SexType female = new SexType();

			//male.Id = 1;
			male.TypeName = "Male";

			//female.Id = 2;
			female.TypeName = "Female";

			s.Save(male);
			s.Save(female);

			Parent bobJones = new Parent();
			bobJones.Id = 1;
			bobJones.AdultName = "Bob Jones";

			Parent maryJones = new Parent();
			maryJones.Id = 2;
			maryJones.AdultName = "Mary Jones";

			Parent charlieSmith = new Parent();
			charlieSmith.Id = 3;
			charlieSmith.AdultName = "Charlie Smith";

			Parent cindySmith = new Parent();
			cindySmith.Id = 4;
			cindySmith.AdultName = "Cindy Smith";

			bobJones.AddFriend(cindySmith);
			bobJones.AddFriend(charlieSmith);
			bobJones.AddFriend(maryJones);
			maryJones.AddFriend(cindySmith);

			s.Save(bobJones, bobJones.Id);
			s.Save(maryJones, maryJones.Id);
			s.Save(charlieSmith, charlieSmith.Id);
			s.Save(cindySmith, cindySmith.Id);

			Child johnnyJones = new Child();
			Child amyJones = new Child();
			Child brianSmith = new Child();
			Child sarahSmith = new Child();

			johnnyJones.Id = 1;
			johnnyJones.FullName = "Johnny Jones";
			johnnyJones.Dad = bobJones;
			johnnyJones.Mom = maryJones;
			johnnyJones.Sex = male;
			johnnyJones.Friends = new Child[] {brianSmith, sarahSmith};
			johnnyJones.FavoriteDate = DateTime.Parse("2003-08-16");

			amyJones.Id = 2;
			amyJones.FullName = "Amy Jones";
			amyJones.Dad = bobJones;
			amyJones.Mom = maryJones;
			amyJones.Sex = female;
			amyJones.FirstSibling = johnnyJones;
			amyJones.Friends = new Child[] {johnnyJones, sarahSmith};

			brianSmith.Id = 11;
			brianSmith.FullName = "Brian Smith";
			brianSmith.Dad = charlieSmith;
			brianSmith.Mom = cindySmith;
			brianSmith.Sex = male;
			brianSmith.Friends = new Child[] {johnnyJones, amyJones, sarahSmith};

			sarahSmith.Id = 12;
			sarahSmith.FullName = "Sarah Smith";
			sarahSmith.Dad = charlieSmith;
			sarahSmith.Mom = cindySmith;
			sarahSmith.Sex = female;
			sarahSmith.Friends = new Child[] {brianSmith};

			Team royals = new Team();
			royals.Name = "Royals";

			Team chiefs = new Team();
			chiefs.Name = "Chiefs";

			royals.Players = new ArrayList();
			royals.Players.Add(amyJones);
			royals.Players.Add(brianSmith);

			chiefs.Players = new ArrayList();
			chiefs.Players.Add(johnnyJones);
			chiefs.Players.Add(sarahSmith);

			s.Save(johnnyJones, johnnyJones.Id);
			s.Save(amyJones, amyJones.Id);
			s.Save(brianSmith, brianSmith.Id);
			s.Save(sarahSmith, sarahSmith.Id);

			s.Save(royals);
			s.Save(chiefs);

			t.Commit();
			s.Close();
		}
	}
}