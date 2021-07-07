using System.Diagnostics;

namespace NHibernate.Test.NHSpecificTest.NH3972
{
	[DebuggerDisplay("{Description}")]
	public class DataState : Entity
	{
		public virtual DataRecordType Type { get; set; }
		public virtual string Description { get; set; }
	}
}
