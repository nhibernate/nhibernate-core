namespace NHibernate.Test.NHSpecificTest.NH3972
{
	public enum ProblemInternalState { New, Closed }

	public class DataProblemState : DataState
	{
		public DataProblemState()
		{
			Type = DataRecordType.Problem;
		}

		public virtual ProblemInternalState State { get; set; }
	}
}
