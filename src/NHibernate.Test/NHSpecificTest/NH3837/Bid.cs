using System;

namespace NHibernate.Test.NHSpecificTest.NH3837
{
	public class Bid
	{
		public virtual Guid Id { get; set; }
		public virtual int Version { get; private set; }
		public virtual string Description { get; set; }
		public virtual Item Item { get; set; }
	}
}
