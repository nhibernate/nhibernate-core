using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH3334
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			Sfi.Statistics.IsStatisticsEnabled = true;
		}

		protected override void OnTearDown()
		{
			Sfi.Statistics.IsStatisticsEnabled = false;
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.CreateQuery("delete from ChildEntity").ExecuteUpdate();
				session.CreateQuery("delete from GrandChildEntity").ExecuteUpdate();
				session.CreateQuery("delete from Entity").ExecuteUpdate();
				session.CreateQuery("delete from OtherEntity").ExecuteUpdate();

				transaction.Commit();
			}
		}

		[Test]
		public void NoExceptionOnExecuteQuery()
		{
			using (var session = OpenSession())
			using (var t = session.BeginTransaction())
			{
				var parent = new Entity
				{
					Name = "Parent1",
					Children = { new ChildEntity { Name = "Child", Child = new GrandChildEntity { Name = "GrandChild" } } }
				};
				session.Save(parent);
				parent = new Entity
				{
					Name = "Parent2",
					Children = { new ChildEntity { Name = "Child", Child = new GrandChildEntity { Name = "XGrandChild" } } }
				};
				var other = new OtherEntity { Name = "ABC", Entities = {parent}};
				parent.OtherEntity = other;
				session.Save(parent);
				session.Save(other);
				t.Commit();
			}

			using (var session = OpenSession())
			using (var _ = session.BeginTransaction())
			{
				var q = session.CreateQuery(
					@"
					SELECT ROOT 
					FROM Entity AS ROOT 
					WHERE
						EXISTS 
							(FROM ELEMENTS(ROOT.Children) AS child
								LEFT JOIN child.Child AS grandChild
								LEFT JOIN ROOT.OtherEntity AS otherEntity
							WHERE
								grandChild.Name like 'G%'
								OR otherEntity.Name like 'G%'
							)");
				Assert.AreEqual(1, q.List().Count);
				
				q = session.CreateQuery(
					@"
					SELECT ROOT 
					FROM Entity AS ROOT 
					WHERE
						EXISTS 
							(FROM ELEMENTS(ROOT.Children) AS child
								LEFT JOIN child.Child AS grandChild
								LEFT JOIN ROOT.OtherEntity AS otherEntity
							WHERE
								grandChild.Name like 'A%'
								OR otherEntity.Name like 'A%'
							)");
				Assert.AreEqual(1, q.List().Count);
				
				/* does not work because of inner join or theta join created for many-to-one
				q = session.CreateQuery(
					@"
					SELECT ROOT 
					FROM Entity AS ROOT 
					WHERE
						EXISTS 
							(FROM ELEMENTS(ROOT.Children) AS child
							WHERE
								child.Child.Name like 'G%'
								OR ROOT.OtherEntity.Name like 'A%'
							)");
				Assert.AreEqual(1, q.List().Count);*/
			}
		}
	}
}
