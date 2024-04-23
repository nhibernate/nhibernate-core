using System;

namespace NHibernate.Test.BulkManipulation
{
	[Serializable]
	public class Enrolment
	{
		public virtual long EnrolmentId { get; set; }
		public virtual Student Student { get; set; }
		public virtual Course Course { get; set; }
	}
}
