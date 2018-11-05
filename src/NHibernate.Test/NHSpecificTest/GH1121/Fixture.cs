using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1121
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Save(new Entity
				{
					Name = "Bob",
					Color = 2
				});

				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.CreateQuery("delete from Entity").ExecuteUpdate();

				transaction.Commit();
			}
		}

		[Test]
		public void CanCastToDifferentType()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = (
					from e in session.Query<Entity>()
					where e.Name == "Bob"
					select new
					{
						e.Id,
						MyColor = (int) e.Color
					}).ToList();

				Assert.That(result, Has.Count.EqualTo(1));
				Assert.That(result[0].MyColor, Is.EqualTo(2));
			}
		}
		
		[Test]
		public void CanCastEnumWithDifferentUnderlyingType()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = (
					from e in session.Query<Entity>()
					where e.Name == "Bob"
					select new
					{
						e.Id,
						MyColor = (Colors) e.Color
					}).ToList();

				Assert.That(result, Has.Count.EqualTo(1));
				Assert.That(result[0].MyColor, Is.EqualTo(Colors.Green));
			}
		}
	}
}
