namespace NHibernate.Test.NHSpecificTest.NH3972
{
	public class Incident : DataRecord
	{
		public Incident()
		{
			Type = DataRecordType.Incident;
		}

		public virtual DataIncidentState State { get; set; }

		public virtual string ReportedBy { get; set; }
	}
}
