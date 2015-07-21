using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH1144
{
	public class MainClass
	{
		private int id;
		private string description;
		
		public MainClass(){}
		
		public MainClass(string description)
		{
			this.description = description;
		}

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual string Description
		{
			get { return description; }
			set { description = value; }
		}
	}
}
