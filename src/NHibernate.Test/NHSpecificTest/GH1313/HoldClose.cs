using System;

namespace NHibernate.Test.NHSpecificTest.GH1313
{
	public class HoldClose
	{
		public virtual int Id { get; set; }
		public virtual Account Account { get; set; }
		public virtual DateTime CloseDate { get; set; }
	}
}
