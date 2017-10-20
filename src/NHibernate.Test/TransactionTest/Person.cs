using System;

namespace NHibernate.Test.TransactionTest
{
	public class PersonBase
	{
		public virtual int Id { get; set; }

		public virtual DateTime CreatedAt { get; set; } = DateTime.Now;

		public virtual string NotNullData { get; set; } = "not-null";
	}

	public class Person : PersonBase
	{ }

	public class CacheablePerson : PersonBase
	{ }
}