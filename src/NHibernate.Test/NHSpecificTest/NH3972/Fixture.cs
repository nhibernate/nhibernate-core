using System;
using System.Linq;
using NHibernate.Cfg;
using NHibernate.Linq;
using NUnit.Framework;
using static NUnit.Framework.Assert;

namespace NHibernate.Test.NHSpecificTest.NH3972
{
	[TestFixture] 
	public class Fixture : BugTestCase
	{ 
		private DataChangeState changeState = null;
		private DataRequestForChangeState rfcState = null;
		private DataIncidentState incidentState = null;
		private DataProblemState problemState = null;

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				foreach (ChangeInternalState state in Enum.GetValues(typeof(ChangeInternalState)))
				{
					changeState = new DataChangeState { State = state, Description = Enum.GetName(typeof(ChangeInternalState), state) };
					session.Save(changeState);
				}

				foreach (RequestForChangeInternalState state in Enum.GetValues(typeof(RequestForChangeInternalState)))
				{
					rfcState = new DataRequestForChangeState { State = state, Description = Enum.GetName(typeof(RequestForChangeInternalState), state) };
					session.Save(rfcState);
				}

				foreach (IncidentInternalState state in Enum.GetValues(typeof(IncidentInternalState)))
				{
					incidentState = new DataIncidentState { State = state, Description = Enum.GetName(typeof(IncidentInternalState), state) };
					session.Save(incidentState);
				}

				foreach (ProblemInternalState state in Enum.GetValues(typeof(ProblemInternalState)))
				{
					problemState = new DataProblemState { State = state, Description = Enum.GetName(typeof(ProblemInternalState), state) };
					session.Save(problemState);
				}

				session.Save(new RequestForChange { Subject = "I have a request", State = rfcState });
				session.Save(new Change { Subject = "I have changed the following stuff", State = changeState, ExecutedBy = "Me" });
				session.Save(new Incident { Subject = "Can someone look for this", State = incidentState, ReportedBy = "Someone" });
				session.Save(new Problem { Subject = "We have a problem", State = problemState });

				session.Flush();
				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");

				session.Flush();
				transaction.Commit();
			}
		}

		[Test]
		public void QueryingAll()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.Query<DataRecord>().ToArray();
				That(result.Length, Is.EqualTo(4));
			}
		}

		[Test]
		public void QueryingSubPropertiesWithDifferentNames()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.Query<DataRecord>().Select(x => new
				{
					x.Subject,
					((Incident)x).ReportedBy,
					((Change)x).ExecutedBy
				}).ToArray();
				That(result.Length, Is.EqualTo(4));
				That(result.Count(x => x.ReportedBy == "Someone") == 1, Is.True); // there is one entity with a set ReportedBy column, i.e. the entity of type "Incident"
				That(result.Count(x => x.ExecutedBy == "Me") == 1, Is.True); // there is one entity with a set ExecutedBy column, i.e. the entity of type "Change"
			}
		}

		[Test]
		public void QueryingSubPropertyWithTheSameNames()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.Query<DataRecord>().Select(x => new
				{
					x.Subject,
					IncidentState = ((Incident)x).State.Description
				}).ToArray();
				That(result.Length, Is.EqualTo(4));
				That(result.Count(x => x.IncidentState == incidentState.Description) == 1, Is.True);
			}
		}
		
		[Test]
		public void QueryingSubPropertiesWithTheSameNames()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.Query<DataRecord>().Select(x => new
				{
					x.Subject,
					IncidentState = ((Incident)x).State.Description,
					ChangeState = ((Change)x).State.Description
				}).ToArray();
				That(result.Length, Is.EqualTo(4));
				That(result.Count(x => x.IncidentState == incidentState.Description) == 1, Is.True); // there is only one "Incident" entity
				That(result.Count(x => x.ChangeState == changeState.Description) == 1, Is.True); // there is only one "Change" entity
			}
		}
	}
}
