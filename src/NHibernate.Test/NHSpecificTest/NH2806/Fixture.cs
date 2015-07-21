using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2806
{
	[TestFixture, Ignore("Not fixed yet.")]
	public  class Fixture : BugTestCase
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
		public void SelectPregnantStatusOfTypeHql()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var list = session.CreateQuery("select a.Pregnant from Animal a where a.class in ('MAMMAL')").List<bool>();

				var count = list.Count();
				Assert.AreEqual(0, count);
			}
		}

		[Test]
		public void SelectAllAnimalsShouldPerformJoins()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				using (var spy = new SqlLogSpy())
				{
					var list = session.CreateQuery("from Animal").List<Animal>();
					var count = list.Count();
					Assert.AreEqual(3, count);
					Assert.Greater(1, spy.GetWholeLog().Split(new[] {"inner join"}, StringSplitOptions.None).Count());
				}
			}
		}
	}
}
