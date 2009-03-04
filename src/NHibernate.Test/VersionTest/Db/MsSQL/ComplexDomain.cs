using Iesi.Collections.Generic;

namespace NHibernate.Test.VersionTest.Db.MsSQL
{
	public class Bar
	{
		public virtual int Id { get; set; }
		public virtual byte[] Timestamp { get; private set; }
		public virtual int AField { get; set; }
		public virtual Foo Foo { get; set; }
	}

	public class Foo
	{
		public Foo()
		{
			Bars = new HashedSet<Bar>();
		}
		public virtual int Id { get; set; }
		public virtual byte[] Timestamp { get; private set; }
		public virtual int AField { get; set; }
		public virtual ISet<Bar> Bars { get; set; }

		public virtual void AddBar(Bar bar)
		{
			bar.Foo = this;
			Bars.Add(bar);
		}
	}
}