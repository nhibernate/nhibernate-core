namespace NHibernate.Test.NHSpecificTest.NH3972
{
	public class RequestForChange : DataRecord
	{
		public RequestForChange()
		{
			Type = DataRecordType.RequestForChange;
		}

		public virtual DataRequestForChangeState State { get; set; }
	}
}
