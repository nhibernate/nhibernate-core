﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH2614
{
	using System.Threading.Tasks;
	[TestFixture]
	public class FixtureAsync : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Save(new ConcreteClass1 {Name = "C11"});
				s.Save(new ConcreteClass1 {Name = "C12"});
				s.Save(new ConcreteClass2 {Name = "C21"});
				s.Save(new ConcreteClass2 {Name = "C22"});
				s.Save(new ConcreteClass2 {Name = "C23"});
				s.Save(new ConcreteClass2 {Name = "C24"});
				t.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.CreateQuery("delete from System.Object").ExecuteUpdate();
				t.Commit();
			}
		}

		[Test]
		public async Task PolymorphicListReturnsCorrectResultsAsync()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				var query = s.CreateQuery(
					@"SELECT Name FROM NHibernate.Test.NHSpecificTest.GH2614.BaseClass ROOT");
				query.SetMaxResults(10);
				var list = await (query.ListAsync());
				Assert.That(list.Count, Is.EqualTo(6));
			}
		}

		[Test]
		public async Task PolymorphicListWithSmallMaxResultsReturnsCorrectResultsAsync()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				var query = s.CreateQuery(
					@"SELECT Name FROM NHibernate.Test.NHSpecificTest.GH2614.BaseClass ROOT");
				query.SetMaxResults(1);
				var list = await (query.ListAsync());
				Assert.That(list.Count, Is.EqualTo(1));
			}
		}

		[Test]
		public async Task PolymorphicListWithSkipReturnsCorrectResultsAsync()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				var query = s.CreateQuery(
					@"SELECT Name FROM NHibernate.Test.NHSpecificTest.GH2614.BaseClass ROOT");
				query.SetFirstResult(5);
				query.SetMaxResults(5);
				var list = await (query.ListAsync());
				Assert.That(list.Count, Is.EqualTo(1));
			}
		}

		[Test]
		public async Task PolymorphicListWithSkipManyReturnsCorrectResultsAsync()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				var query = s.CreateQuery(
					@"SELECT Name FROM NHibernate.Test.NHSpecificTest.GH2614.BaseClass ROOT");
				query.SetFirstResult(6);
				query.SetMaxResults(5);
				var list = await (query.ListAsync());
				Assert.That(list.Count, Is.EqualTo(0));
			}
		}

		[Test]
		public async Task PolymorphicListWithOrderByStillShowsWarningAsync()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				var query = s.CreateQuery(
					@"SELECT Name FROM NHibernate.Test.NHSpecificTest.GH2614.BaseClass ROOT ORDER BY ROOT.Name");
				query.SetMaxResults(3);
				var list = await (query.ListAsync());
				Assert.That(list.Count, Is.EqualTo(3));
			}
		}
	}
}
