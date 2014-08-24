using System;
using System.Collections.Generic;

namespace NHibernate.Test.Cascade
{
	public class A
	{
		private long id;
		private string data;
		private ISet<H> hs; // A 1 - * H
		private G g; // A 1 - 1 G
		
		public A()
		{
			hs = new HashSet<H>();
		}
	
		public A(string data) : this()
		{
			this.data = data;
		}

		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}
	
		public virtual string Data
		{
			get { return data; }
			set { data = value; }
		}
		
		public virtual G G
		{
			get { return g; }
			set { g = value; }
		}
		
		public virtual ISet<H> Hs
		{
			get { return hs; }
			set { hs = value; }
		}

		public virtual void AddH(H h)
		{
			hs.Add(h);
			h.A = this;
		}
	
		public override string ToString()
		{
			return "A[" + id + ", " + data + "]";
		}
	}
}