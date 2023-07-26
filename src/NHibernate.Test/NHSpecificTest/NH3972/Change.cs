namespace NHibernate.Test.NHSpecificTest.NH3972
{
	public class Change : DataRecord
	{
		public Change()
		{
			Type = DataRecordType.Change;
		}

		public virtual DataChangeState State { get; set; }

		public virtual string ExecutedBy { get; set; }
	}
}
