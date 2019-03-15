using System.Collections.Generic;

namespace NHibernate.Test.CollectionTest
{
	public class Env
	{
		public virtual long Id { get; set; }
		public virtual IList<MachineRequest> RequestsFailed { get; set; }
		public virtual IDictionary<long, MachineRequest> FailedRequestsById { get; set; }
	}

	public class MachineRequest
	{
		public virtual long Id { get; set; }
		public virtual int RequestCompletionStatus { get; set; }
		public virtual long EnvId { get; set; }
	}
}
