using System;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3234
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		private void Evict(ISession session, GridWidget widget)
		{
			session.Evict(widget);
			sessions.Evict(widget.GetType());
		}

		private static void Save(ISession session, GridWidget widget)
		{
			if (widget.Id != Guid.Empty && !session.Contains(widget))
				widget = session.Merge(widget);

			session.SaveOrUpdate(widget);
			session.Flush();
		}

		protected override void OnTearDown()
		{
			using (var session=OpenSession())
			using (var tx = session.BeginTransaction())
			{
				session.Delete("from System.Object");
				tx.Commit();
			}
		}

		[Test]
		public void ShouldNotFailWhenAddingNewLevels()
		{
			using (var session = OpenSession())
			{
				var widget = new GridWidget
					{
						Levels =
							{
								new GridLevel(),
								new GridLevel()
							},
					};

				Save(session, widget);
				Evict(session, widget);

				widget.Levels.Add(new GridLevel());

				Save(session, widget);
				Evict(session, widget);

				var loaded = session.Get<GridWidget>(widget.Id);

				Assert.That(loaded.Levels.Count, Is.EqualTo(3));
			}
		}

		[Test]
		public void ShouldNotFailWhenReplacingLevels()
		{
			using (var session = OpenSession())
			{
				var widget = new GridWidget
					{
						Levels =
							{
								new GridLevel(),
								new GridLevel()
							},
					};

				Save(session, widget);
				Evict(session, widget);

				widget.Levels.Clear();
				widget.Levels.Add(new GridLevel());

				Save(session, widget);
				Evict(session, widget);

				var loaded = session.Get<GridWidget>(widget.Id);

				Assert.That(loaded.Levels.Count, Is.EqualTo(1));
			}
		}
	}
}
