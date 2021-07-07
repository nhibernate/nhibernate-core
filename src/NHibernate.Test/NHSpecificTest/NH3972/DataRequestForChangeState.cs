namespace NHibernate.Test.NHSpecificTest.NH3972
{
	public enum RequestForChangeInternalState { New, Orded, Closed, Cancelled }

	public class DataRequestForChangeState : DataState
	{
		public DataRequestForChangeState()
		{
			Type = DataRecordType.RequestForChange;
		}

		public virtual RequestForChangeInternalState State { get; set; }
	}
}
