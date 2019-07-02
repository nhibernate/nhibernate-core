using System;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class EmailAddress
	{
		public virtual int Id { get; set; }
		public virtual string Email { get; set; }
		public virtual DateTime ModifiedDate { get; set; }
		public virtual Guid RowGuid { get; set; }

		public virtual Person BusinessEntity { get; set; }
	}
}
