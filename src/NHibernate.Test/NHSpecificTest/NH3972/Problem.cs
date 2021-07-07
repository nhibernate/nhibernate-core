namespace NHibernate.Test.NHSpecificTest.NH3972
{
	public class Problem : DataRecord
	{
		public Problem()
		{
			Type = DataRecordType.Problem;
		}

		public virtual DataProblemState State { get; set; }
	}
}
