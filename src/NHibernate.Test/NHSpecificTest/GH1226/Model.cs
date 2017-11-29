using System;

namespace NHibernate.Test.NHSpecificTest.GH1226
{
	public class Account
	{
		public virtual Guid Id { get; set; }
		public virtual Bank Bank { get; set; }
	}

	public class Bank
	{
		public virtual Guid Id { get; set; }
		public virtual string Code { get; set; }
	}
}
