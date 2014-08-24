using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.Properties
{
	public class Person
	{
		public virtual long Id { get; set; }
		public virtual string Name { get; set; }
		public virtual Address Address { get; set; }
		public virtual string UserId { get; set; }
		public virtual bool Deleted { get; set; }
		public virtual ISet<Account> Accounts { get; set; }

		public Person()
		{
			Accounts = new HashSet<Account>();
		}
	}

	public class Address
	{
		public virtual long Id { get; set; }
		public virtual string Addr { get; set; }
		public virtual string Zip { get; set; }
		public virtual string Country { get; set; }
		public virtual Person Person { get; set; }
	}

	public class Account
	{
		public virtual string AccountId { get; set; }
		public virtual Person User { get; set; }
		public virtual char Type { get; set; }
	}

	public class DynamicEntity
	{
		public virtual long Id { get; set; }
		public virtual string Foo { get; set; }
		public virtual string Bar { get; set; }
	}
}
