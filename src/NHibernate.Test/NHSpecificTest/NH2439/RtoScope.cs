using System;

namespace NHibernate.Test.NHSpecificTest.NH2439
{
	public class RtoScope
	{
		public virtual Guid Id { get; set; }

		public virtual Organisation Rto { get; set; }

		public virtual TrainingComponent Nrt { get; set; }

		public virtual DateTime StartDate { get; set; }

		public virtual DateTime? EndDate { get; set; }

		public virtual bool IsRefused { get; set; }
	}
}
