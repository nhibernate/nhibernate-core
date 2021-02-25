using System;

namespace NHibernate.Test.NHSpecificTest.GH1920
{
	public class EntityWithBatchSize
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
	}
}
