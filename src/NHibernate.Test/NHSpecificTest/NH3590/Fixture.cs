using System;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.NHSpecificTest.NH3590
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		private Entity entity;

		protected override void OnSetUp()
		{
			entity = new Entity();
			using (var s = OpenSession())
			{
				using (var tx = s.BeginTransaction())
				{
					s.Save(entity);
					tx.Commit();
				}
			}
		}

		[Test]
		public void ShouldUpdate()
		{
			entity.Dates.Add(DateTime.Now);
			using (var s = OpenSession())
			{
				using (var tx = s.BeginTransaction())
				{
					s.Update(entity);
					tx.Commit();
				}
			}

			using (var s = OpenSession())
			{
				using (s.BeginTransaction())
				{
					s.Get<Entity>(entity.Id).Dates.Count
						.Should().Be.EqualTo(1);
				}
			}
		}

		[Test]
		public void ShouldMerge()
		{
			entity.Dates.Add(DateTime.Now);
			using (var s = OpenSession())
			{
				using (var tx = s.BeginTransaction())
				{
					s.Merge(entity);
					tx.Commit();
				}
			}

			using (var s = OpenSession())
			{
				using (s.BeginTransaction())
				{
					s.Get<Entity>(entity.Id).Dates.Count
						.Should().Be.EqualTo(1);
				}
			}
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			{
				using (var tx = s.BeginTransaction())
				{
					s.Delete(s.Get<Entity>(entity.Id));
					tx.Commit();
				}
			}
		}
	}
}