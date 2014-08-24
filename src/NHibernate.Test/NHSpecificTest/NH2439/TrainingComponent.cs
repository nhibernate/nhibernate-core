using System;

namespace NHibernate.Test.NHSpecificTest.NH2439
{
	public class TrainingComponent
	{
		public virtual Guid Id { get; set; }
		public virtual string Code { get; set; }
		public virtual string Title { get; set; }
	}
}
