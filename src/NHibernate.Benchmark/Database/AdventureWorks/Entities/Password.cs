using System;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class Password
	{
		public virtual DateTime ModifiedDate { get; set; }
		public virtual string PasswordHash { get; set; }
		public virtual string PasswordSalt { get; set; }
		public virtual Guid RowGuid { get; set; }

		public virtual Person BusinessEntity { get; set; }
	}
}
