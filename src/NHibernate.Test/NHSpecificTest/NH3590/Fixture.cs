using System;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3590
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		private Entity _entity;

		protected override void OnSetUp()
		{
			_entity = new Entity();
			using (var s = OpenSession())
			{
				using (var tx = s.BeginTransaction())
				{
					s.Save(_entity);
					tx.Commit();
				}
			}
		}

		[Test]
		public void ShouldUpdate()
		{
			_entity.Dates.Add(DateTime.Now);
			using (var s = OpenSession())
			{
				using (var tx = s.BeginTransaction())
				{
					s.Update(_entity);
					tx.Commit();
				}
			}

			using (var s = OpenSession())
			{
				using (s.BeginTransaction())
				{
					Assert.That(s.Get<Entity>(_entity.Id).Dates.Count, Is.EqualTo(1));
				}
			}
		}

		[Test]
		public void ShouldMerge()
		{
			_entity.Dates.Add(DateTime.Now);
			using (var s = OpenSession())
			{
				using (var tx = s.BeginTransaction())
				{
					s.Merge(_entity);
					tx.Commit();
				}
			}

			using (var s = OpenSession())
			{
				using (s.BeginTransaction())
				{
					Assert.That(s.Get<Entity>(_entity.Id).Dates.Count, Is.EqualTo(1));
				}
			}
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			{
				using (var tx = s.BeginTransaction())
				{
					s.Delete(s.Get<Entity>(_entity.Id));
					tx.Commit();
				}
			}
		}
	}
}