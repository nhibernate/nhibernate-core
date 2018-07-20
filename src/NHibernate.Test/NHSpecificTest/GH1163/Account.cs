using System;

namespace NHibernate.Test.NHSpecificTest.GH1163
{
	public class Account
	{
		public virtual long? Id { get; set; }
		public virtual string Login { get; set; }
		public virtual string PasswordHash { get; set; }
		public virtual DateTime ValidFrom { get; set; }
		public virtual DateTime? ValidUntil { get; set; }
	}
}
