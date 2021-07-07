namespace NHibernate.Test.NHSpecificTest.NH3972
{
	public enum IncidentInternalState { New, Assigned, Closed, Solved }

	public class DataIncidentState : DataState
	{
		public DataIncidentState()
		{
			Type = DataRecordType.Incident;
		}

		public virtual IncidentInternalState State { get; set; }
	}
}
