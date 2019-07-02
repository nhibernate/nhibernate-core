using System;
using System.Collections.Generic;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class Person
	{
		public virtual int EmailPromotion { get; set; }
		public virtual string FirstName { get; set; }
		public virtual string LastName { get; set; }
		public virtual string MiddleName { get; set; }
		public virtual DateTime ModifiedDate { get; set; }
		public virtual bool NameStyle { get; set; }
		public virtual string PersonType { get; set; }
		public virtual Guid RowGuid { get; set; }
		public virtual string Suffix { get; set; }
		public virtual string Title { get; set; }
		public virtual string AdditionalContactInfo { get; set; }
		public virtual string Demographics { get; set; }

		public virtual ICollection<BusinessEntityContact> BusinessEntityContact { get; set; } =
			new HashSet<BusinessEntityContact>();

		public virtual ICollection<Customer> Customer { get; set; } = new HashSet<Customer>();
		public virtual ICollection<EmailAddress> EmailAddress { get; set; } = new HashSet<EmailAddress>();
		public virtual Employee Employee { get; set; }
		public virtual Password Password { get; set; }
		public virtual ICollection<PersonCreditCard> PersonCreditCard { get; set; } = new HashSet<PersonCreditCard>();
		public virtual ICollection<PersonPhone> PersonPhone { get; set; } = new HashSet<PersonPhone>();
		public virtual BusinessEntity BusinessEntity { get; set; }
	}
}
