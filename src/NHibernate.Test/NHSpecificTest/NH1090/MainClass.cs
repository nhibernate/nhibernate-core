using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH1090
{
	public class MainClass
	{
		private int id;
		private string description;
		private string title;

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
		public virtual string Title
		{
			get { return title; }
			set { title = value; }
		}
	}
}