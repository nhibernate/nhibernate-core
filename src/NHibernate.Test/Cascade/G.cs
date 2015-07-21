using System;
using System.Collections.Generic;

namespace NHibernate.Test.Cascade
{
	public class G
	{
		private long id;
		private string data;
		private A a; // A 1 <-> 1 G
		private ISet<H> hs; // G * <-> * H
		
		public G() : this(null)
		{
		}
	
		public G(string data)
		{
			this.data = data;
			hs = new HashSet<H>();
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
		
		public virtual ISet<H> Hs
		{
			get { return hs; }
			set { hs = value; }
		}
	}
}