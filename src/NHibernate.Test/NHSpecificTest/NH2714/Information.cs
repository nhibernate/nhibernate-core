using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH2714
{
	public class Information
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual int ExtraId { get; set; }
		public virtual ISet<Item> Items { get; set; } = new HashSet<Item>();
	}
}
