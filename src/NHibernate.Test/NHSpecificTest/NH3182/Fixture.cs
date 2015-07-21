using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3182
{
	[TestFixture, Ignore("Not fixed yet.")]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			var mother = new Lizard
				{
					BodyWeight = 48,
					Description = "Mother",
					Children = new List<Animal>()
				};
			var father = new Lizard
				{
					BodyWeight = 48,
					Description = "Father",
					Children = new List<Animal>()
				};
			var child = new Lizard
				{
					Mother = mother,
					Father = father,
					BodyWeight = 48,
					Description = "Child",
				};

			mother.Children.Add(child);
			father.Children.Add(child);

			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Save(mother);
				session.Save(father);
				session.Save(child);

				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");

				transaction.Commit();
			}
		} 

		[Test]
		public void SelectManyPregnantStatusCast1()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var list = (session.Query<Animal>()
					.SelectMany(o => o.Children)
					.Where(o => o is Mammal)
					.Select(o => ((Mammal) o).Pregnant))
					.ToList();

				var count = list.Count();
				Assert.AreEqual(0, count);
			}
		}

		[Test]
		public void SelectManyPregnantStatusCast2()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var list = (session.Query<Animal>()
					.SelectMany(o => o.Children)
					.Where(o => o is Mammal)
					.Select(o => ((Mammal) o).Pregnant))
					.ToList();

				var count = list.Count();
				Assert.AreEqual(0, count);
			}
		}

		[Test]
		public void SelectManyPregnantStatusOfType1()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var list = session.Query<Animal>()
					.SelectMany(o => o.Children, (animal, animal1) => animal1)
					.OfType<Mammal>()
					.Select(o => o.Pregnant)
					.ToList();

				var count = list.Count();
				Assert.AreEqual(0, count);
			}
		}

		[Test]
		public void SelectManyPregnantStatusOfType2()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var list = session.Query<Animal>()
					.SelectMany(o => o.Children, (animal, animal1) => animal1)
					.OfType<Mammal>()
					.Select(o => o.Pregnant)
					.ToList();

				var count = list.Count();
				Assert.AreEqual(0, count);
			}
		}

		[Test]
		public void SelectPregnantStatusCast()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var list = (session.Query<Animal>()
					.Where(o => o is Mammal)
					.Select(o => ((Mammal)o).Pregnant))
					.ToList();

				var count = list.Count();
				Assert.AreEqual(0, count);
			}
		}

		[Test]
		public void SelectPregnantStatusOfType()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var list = session.Query<Animal>()
					.OfType<Mammal>()
					.Select(o => o.Pregnant)
					.ToList();

				var count = list.Count();
				Assert.AreEqual(0, count);
			}
		}

		[Test]
		public void SelectPregnantStatus()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var list = session.Query<Mammal>()
					.Select(o => o.Pregnant)
					.ToList();

				var count = list.Count();
				Assert.AreEqual(0, count);
			}
		}
	}
}
