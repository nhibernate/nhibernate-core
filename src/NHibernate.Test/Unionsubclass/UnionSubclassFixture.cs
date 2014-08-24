using System.Collections;
using System.Collections.Generic;
using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.Unionsubclass
{
	[TestFixture]
	public class UnionSubclassFixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new string[] { "Unionsubclass.Beings.hbm.xml" }; }
		}

		[Test]
		public void UnionSubclassCollection()
		{
			using (ISession s = OpenSession())
			{
				using (ITransaction t = s.BeginTransaction())
				{
					Location mel = new Location("Earth");
					s.Save(mel);

					Human gavin = new Human();
					gavin.Identity = "gavin";
					gavin.Sex = 'M';
					gavin.Location = mel;
					mel.AddBeing(gavin);

					gavin.Info.Add("bar");
					gavin.Info.Add("y");

					t.Commit();
				}
				s.Close();
			}

			using (ISession s = OpenSession())
			{
				using (ITransaction t = s.BeginTransaction())
				{
					Human gavin = (Human)s.CreateCriteria(typeof(Human)).UniqueResult();
					Assert.AreEqual(gavin.Info.Count, 2);
					s.Delete(gavin);
					s.Delete(gavin.Location);
					t.Commit();
				}
				s.Close();
			}
		}

		[Test]
		public void UnionSubclassFetchMode()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Location mel = new Location("Earth");
			s.Save(mel);

			Human gavin = new Human();
			gavin.Identity = "gavin";
			gavin.Sex = 'M';
			gavin.Location = mel;
			mel.AddBeing(gavin);
			Human max = new Human();
			max.Identity = "max";
			max.Sex = 'M';
			max.Location = mel;
			mel.AddBeing(gavin);

			s.Flush();
			s.Clear();

			IList list =
				s.CreateCriteria(typeof(Human)).SetFetchMode("location", FetchMode.Join).SetFetchMode("location.beings",
																																															 FetchMode.Join).List();

			for (int i = 0; i < list.Count; i++)
			{
				Human h = (Human)list[i];
				Assert.IsTrue(NHibernateUtil.IsInitialized(h.Location));
				Assert.IsTrue(NHibernateUtil.IsInitialized(h.Location.Beings));
				s.Delete(h);
			}
			s.Delete(s.Get<Location>(mel.Id));
			t.Commit();
			s.Close();
		}

		[Test]
		public void UnionSubclassOneToMany()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Location mel = new Location("Melbourne, Australia");
			Location mars = new Location("Mars");
			s.Save(mel);
			s.Save(mars);

			Human gavin = new Human();
			gavin.Identity = "gavin";
			gavin.Sex = 'M';
			gavin.Location = mel;
			mel.AddBeing(gavin);

			Alien x23y4 = new Alien();
			x23y4.Identity = "x23y4$$hu%3";
			x23y4.Location = mars;
			x23y4.Species = "martian";
			mars.AddBeing(x23y4);

			Alien yy3dk = new Alien();
			yy3dk.Identity = "yy3dk&*!!!";
			yy3dk.Location = mars;
			yy3dk.Species = "martian";
			mars.AddBeing(yy3dk);

			Hive hive = new Hive();
			hive.Location = mars;
			hive.Members.Add(x23y4);
			x23y4.Hive = hive;
			hive.Members.Add(yy3dk);
			yy3dk.Hive = hive;
			s.Persist(hive);

			yy3dk.Hivemates.Add(x23y4);
			x23y4.Hivemates.Add(yy3dk);

			s.Flush();
			s.Clear();

			hive = (Hive)s.CreateQuery("from Hive h").UniqueResult();
			Assert.IsFalse(NHibernateUtil.IsInitialized(hive.Members));
			Assert.AreEqual(2, hive.Members.Count);

			s.Clear();

			hive = (Hive)s.CreateQuery("from Hive h left join fetch h.location left join fetch h.members").UniqueResult();
			Assert.IsTrue(NHibernateUtil.IsInitialized(hive.Members));
			Assert.AreEqual(2, hive.Members.Count);

			s.Clear();

			x23y4 = (Alien)s.CreateQuery("from Alien a left join fetch a.hivemates where a.identity like 'x%'").UniqueResult();
			Assert.IsTrue(NHibernateUtil.IsInitialized(x23y4.Hivemates));
			Assert.AreEqual(1, x23y4.Hivemates.Count);

			s.Clear();

			x23y4 = (Alien)s.CreateQuery("from Alien a where a.identity like 'x%'").UniqueResult();
			Assert.IsFalse(NHibernateUtil.IsInitialized(x23y4.Hivemates));
			Assert.AreEqual(1, x23y4.Hivemates.Count);

			s.Clear();

			x23y4 = (Alien)s.CreateCriteria(typeof(Alien)).AddOrder(Order.Asc("identity")).List()[0];
			s.Delete(x23y4.Hive);
			s.Delete(s.Get<Location>(mel.Id));
			s.Delete(s.Get<Location>(mars.Id));
			Assert.IsTrue(s.CreateQuery("from Being").List().Count == 0);
			t.Commit();
			s.Close();
		}

		[Test]
		public void UnionSubclassManyToOne()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Location mel = new Location("Melbourne, Australia");
			Location mars = new Location("Mars");
			s.Save(mel);
			s.Save(mars);

			Human gavin = new Human();
			gavin.Identity = "gavin";
			gavin.Sex = 'M';
			gavin.Location = mel;
			mel.AddBeing(gavin);

			Alien x23y4 = new Alien();
			x23y4.Identity = "x23y4$$hu%3";
			x23y4.Location = mars;
			x23y4.Species = "martian";
			mars.AddBeing(x23y4);

			Hive hive = new Hive();
			hive.Location = mars;
			hive.Members.Add(x23y4);
			x23y4.Hive = hive;
			s.Persist(hive);

			Thing thing = new Thing();
			thing.Description = "some thing";
			thing.Owner = gavin;
			gavin.Things.Add(thing);
			s.Save(thing);
			s.Flush();

			s.Clear();

			thing = (Thing)s.CreateQuery("from Thing t left join fetch t.owner").UniqueResult();
			Assert.IsTrue(NHibernateUtil.IsInitialized(thing.Owner));
			Assert.AreEqual("gavin", thing.Owner.Identity);
			s.Clear();

			thing = (Thing)s.CreateQuery("select t from Thing t left join t.owner where t.owner.identity='gavin'").UniqueResult();
			Assert.IsFalse(NHibernateUtil.IsInitialized(thing.Owner));
			Assert.AreEqual("gavin", thing.Owner.Identity);
			s.Clear();

			gavin = (Human)s.CreateQuery("from Human h left join fetch h.things").UniqueResult();
			Assert.IsTrue(NHibernateUtil.IsInitialized(gavin.Things));
			Assert.AreEqual("some thing", ((Thing)gavin.Things[0]).Description);
			s.Clear();

			Assert.AreEqual(2, s.CreateQuery("from Being b left join fetch b.things").List().Count);
			s.Clear();

			gavin = (Human)s.CreateQuery("from Being b join fetch b.things").UniqueResult();
			Assert.IsTrue(NHibernateUtil.IsInitialized(gavin.Things));
			Assert.AreEqual("some thing", ((Thing)gavin.Things[0]).Description);
			s.Clear();

			gavin = (Human)s.CreateQuery("select h from Human h join h.things t where t.description='some thing'").UniqueResult();
			Assert.IsFalse(NHibernateUtil.IsInitialized(gavin.Things));
			Assert.AreEqual("some thing", ((Thing)gavin.Things[0]).Description);
			s.Clear();

			gavin = (Human)s.CreateQuery("select b from Being b join b.things t where t.description='some thing'").UniqueResult();
			Assert.IsFalse(NHibernateUtil.IsInitialized(gavin.Things));
			Assert.AreEqual("some thing", ((Thing)gavin.Things[0]).Description);
			s.Clear();

			thing = s.Get<Thing>(thing.Id);
			Assert.IsFalse(NHibernateUtil.IsInitialized(thing.Owner));
			Assert.AreEqual("gavin", thing.Owner.Identity);

			thing.Owner.Things.Remove(thing);
			thing.Owner = x23y4;
			x23y4.Things.Add(thing);

			s.Flush();
			s.Clear();

			thing = s.Get<Thing>(thing.Id);
			Assert.IsFalse(NHibernateUtil.IsInitialized(thing.Owner));
			Assert.AreEqual("x23y4$$hu%3", thing.Owner.Identity);

			s.Delete(thing);
			x23y4 = (Alien)s.CreateCriteria(typeof(Alien)).UniqueResult();
			s.Delete(x23y4.Hive);
			s.Delete(s.Get<Location>(mel.Id));
			s.Delete(s.Get<Location>(mars.Id));
			Assert.AreEqual(0, s.CreateQuery("from Being").List().Count);
			t.Commit();
			s.Close();
		}

		[Test]
		public void UnionSubclass()
		{
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				Location mel = new Location("Melbourne, Australia");
				Location atl = new Location("Atlanta, GA");
				Location mars = new Location("Mars");
				s.Save(mel);
				s.Save(atl);
				s.Save(mars);

				Human gavin = new Human();
				gavin.Identity = "gavin";
				gavin.Sex = 'M';
				gavin.Location = mel;
				mel.AddBeing(gavin);

				Alien x23y4 = new Alien();
				x23y4.Identity = "x23y4$$hu%3";
				x23y4.Location = mars;
				x23y4.Species = "martian";
				mars.AddBeing(x23y4);

				Hive hive = new Hive();
				hive.Location = mars;
				hive.Members.Add(x23y4);
				x23y4.Hive = hive;
				s.Persist(hive);

				Assert.AreEqual(2, s.CreateQuery("from Being").List().Count);
				Assert.AreEqual(1, s.CreateQuery("from Being b where b.class = Alien").List().Count);
				Assert.AreEqual(1, s.CreateQuery("from Alien").List().Count);
				s.Clear();

				IList<Being> beings = s.CreateQuery("from Being b left join fetch b.location").List<Being>();
				foreach (Being b in beings)
				{
					Assert.IsTrue(NHibernateUtil.IsInitialized(b.Location));
					Assert.IsNotNull(b.Location.Name);
					Assert.IsNotNull(b.Identity);
					Assert.IsNotNull(b.Species);
				}
				Assert.AreEqual(2, beings.Count);
				s.Clear();

				beings = s.CreateQuery("from Being").List<Being>();
				foreach (Being b in beings)
				{
					Assert.IsFalse(NHibernateUtil.IsInitialized(b.Location));
					Assert.IsNotNull(b.Location.Name);
					Assert.IsNotNull(b.Identity);
					Assert.IsNotNull(b.Species);
				}
				Assert.AreEqual(2, beings.Count);
				s.Clear();

				IList<Location> locations = s.CreateQuery("from Location").List<Location>();
				int count = 0;
				foreach (Location l in locations)
				{
					Assert.IsNotNull(l.Name);
					foreach (Being o in l.Beings)
					{
						count++;
						Assert.AreSame(o.Location, l);
					}
				}
				Assert.AreEqual(2, count);
				Assert.AreEqual(3, locations.Count);
				s.Clear();

				locations = s.CreateQuery("from Location loc left join fetch loc.beings").List<Location>();
				count = 0;
				foreach (Location l in locations)
				{
					Assert.IsNotNull(l.Name);
					foreach (Being o in l.Beings)
					{
						count++;
						Assert.AreSame(o.Location, l);
					}
				}

				Assert.AreEqual(2, count);
				Assert.AreEqual(3, locations.Count);
				s.Clear();

				gavin = s.Get<Human>(gavin.Id);
				atl = s.Get<Location>(atl.Id);

				atl.AddBeing(gavin);
				Assert.AreEqual(1, s.CreateQuery("from Human h where h.location.name like '%GA'").List().Count);
				s.Delete(gavin);
				x23y4 = (Alien) s.CreateCriteria(typeof (Alien)).UniqueResult();
				s.Delete(x23y4.Hive);
				Assert.AreEqual(0, s.CreateQuery("from Being").List().Count);
				t.Commit();
				s.Close();
			}
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Delete("from Location");
				t.Commit();
			}
		}

		[Test]
		public void NestedUnionedSubclasses()
		{
			ISession s;
			ITransaction tx;
			s = OpenSession();
			tx = s.BeginTransaction();
			Location mel = new Location("Earth");
			Human marcf = new Human();
			marcf.Identity = "marc";
			marcf.Sex = 'M';
			mel.AddBeing(marcf);
			Employee steve = new Employee();
			steve.Identity = "steve";
			steve.Sex = 'M';
			steve.Salary = 0;
			mel.AddBeing(steve);
			s.Persist(mel);
			tx.Commit();
			s.Close();
			s = OpenSession();
			tx = s.BeginTransaction();
			IQuery q = s.CreateQuery("from Being h where h.identity = :name1 or h.identity = :name2");
			q.SetString("name1", "marc");
			q.SetString("name2", "steve");
			IList result = q.List();
			Assert.AreEqual(2, result.Count);
			s.Delete(result[0]);
			s.Delete(result[1]);
			s.Delete(((Human)result[0]).Location);
			tx.Commit();
			s.Close();
		}
	}
}