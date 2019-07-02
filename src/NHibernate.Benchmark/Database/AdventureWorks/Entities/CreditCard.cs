using System;
using System.Collections.Generic;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class CreditCard
	{
		public virtual int Id { get; set; }
		public virtual string CardNumber { get; set; }
		public virtual string CardType { get; set; }
		public virtual byte ExpMonth { get; set; }
		public virtual short ExpYear { get; set; }
		public virtual DateTime ModifiedDate { get; set; }

		public virtual ICollection<PersonCreditCard> PersonCreditCard { get; set; } = new HashSet<PersonCreditCard>();
		public virtual ICollection<SalesOrderHeader> SalesOrderHeader { get; set; } = new HashSet<SalesOrderHeader>();
	}
}
