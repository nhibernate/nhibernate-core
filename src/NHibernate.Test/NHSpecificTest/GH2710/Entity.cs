using System;

namespace NHibernate.Test.NHSpecificTest.GH2710
{
	public class Entity
	{
		public virtual int Id { get; set; }
		public virtual int MbrId { get; set; }
		public virtual string MrcDailyMoved { get; set; } = "N";
	}
}
