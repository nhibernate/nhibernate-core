using System;
using System.Collections.Generic;

namespace NHibernate.Test.Cascade
{
	public class H
	{
		private long id;
		private string data;
		private A a;
		private ISet<G> gs; // G * <-> * H
		
		public H() : this(null)
		{
		}
	
		public H(string data)
		{
			this.data = data;
			gs = new HashSet<G>();
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

		public virtual A A
		{
			get { return a; }
			set { a = value; }
		}
		
		public virtual ISet<G> Gs
		{
			get { return gs; }
			set { gs = value; }
		}
	}
}