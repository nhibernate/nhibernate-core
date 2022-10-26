using System.Diagnostics;

namespace NHibernate.Test.NHSpecificTest.NH3972
{
	public enum DataRecordType
	{
		Incident,
		Problem,
		RequestForChange,
		Change
	}

	[DebuggerDisplay("{Subject}")]
	public class DataRecord : Entity
	{
		public virtual DataRecordType Type { get; set; }
		public virtual string Subject { get; set; }
	}
}
