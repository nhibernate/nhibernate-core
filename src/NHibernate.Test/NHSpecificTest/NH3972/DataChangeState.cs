namespace NHibernate.Test.NHSpecificTest.NH3972
{
	public enum ChangeInternalState { New, Completed }

	public class DataChangeState : DataState
	{
		public DataChangeState()
		{
			Type = DataRecordType.Change;
		}

		public virtual ChangeInternalState State { get; set; }
	}
}
