using System;

namespace NHibernate.Test.NHSpecificTest.NH1835
{
	public class Document
	{
		public virtual Guid Id { get; set; }
		public virtual byte[] Contents { get; set; }
	}
}