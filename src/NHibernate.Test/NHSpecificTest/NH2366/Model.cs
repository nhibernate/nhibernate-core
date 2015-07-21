using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH2366
{
	public class One
	{
		private int id;
		private string value;
		private ISet<Two> twos = new HashSet<Two>();
		
		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}
		
		public virtual string Value
		{
			get { return value; }
			set { this.value = value; }
		}
		
		public virtual ISet<Two> Twos
		{
			get { return twos; }
			set { twos = value; }
		}
		
		public One()
		{
		}
	}
	
	public class Two
	{
		private int id;
		private string value;
		
		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}
		
		public virtual string Value
		{
			get { return value; }
			set { this.value = value; }
		}
		
		public Two()
		{
		}
	}
}
