using System.Collections.Generic;

namespace NHibernate.Test.Operations
{
	public class Competition
	{
		public Competition()
		{
			Competitors = new List<Competitor>();
		}
		public virtual int Id { get; set; }
		public virtual IList<Competitor> Competitors { get; set; }
	}
}