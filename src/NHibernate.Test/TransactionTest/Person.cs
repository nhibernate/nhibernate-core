using System;

namespace NHibernate.Test.TransactionTest
{
	public class Person
	{
		public virtual int Id { get; set; }

		public virtual DateTime CreatedAt { get; set; } = DateTime.Now;
	}
}