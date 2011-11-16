using System;

namespace NHibernate.Test.NHSpecificTest.NH2858
{
	public class DomainClass
	{
		public virtual int Id { get; set; }
		public virtual Guid TheGuid { get; set; }
	}
}