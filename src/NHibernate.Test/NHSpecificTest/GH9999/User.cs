using System;

namespace NHibernate.Test.NHSpecificTest.GH9999
{
	public class User
	{
		public virtual int Id { get; set; }

		public virtual DateTime? Birthday { get; set; }
	}
}
