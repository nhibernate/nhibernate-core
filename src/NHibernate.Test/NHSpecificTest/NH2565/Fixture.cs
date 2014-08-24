using System;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.NHSpecificTest.NH2565
{
	public class Fixture: BugTestCase
	{
		private class TaskSavedScenario: IDisposable
		{
			private readonly ISessionFactory factory;
			private readonly Guid taskId;

			public TaskSavedScenario(ISessionFactory factory)
			{
				this.factory = factory;
				var activity = new TaskActivity{Name="Say Hello!"};
				var task = new Task { Description = "Nice to do", Activity = activity };
				using (var s = factory.OpenSession())
				using (var tx = s.BeginTransaction())
				{
					s.Persist(task);
					taskId = task.Id;
					tx.Commit();
				}
			}

			public Guid TaskId
			{
				get { return taskId; }
			}

			public void Dispose()
			{
				using (var s = factory.OpenSession())
				using (var tx = s.BeginTransaction())
				{
					s.Delete(s.Get<Task>(taskId));
					tx.Commit();
				}
			}
		}

		[Test]
		public void WhenUseLoadThenCanUsePersistToModify()
		{
			using (var scenario = new TaskSavedScenario(Sfi))
			{
				using (var s = OpenSession())
				using (var tx = s.BeginTransaction())
				{
					var task = s.Load<Task>(scenario.TaskId);
					task.Description = "Could be something nice";
					s.Persist(task);
					s.Executing(session => session.Persist(task)).NotThrows();
					tx.Commit();
				}
			}
		}

		[Test]
		public void WhenUseGetThenCanUsePersistToModify()
		{
			using (var scenario = new TaskSavedScenario(Sfi))
			{
				using (var s = OpenSession())
				using (var tx = s.BeginTransaction())
				{
					var task = s.Get<Task>(scenario.TaskId);
					task.Description = "Could be something nice";
					s.Executing(session => session.Persist(task)).NotThrows();
					tx.Commit();
				}
			}
		}
	}
}