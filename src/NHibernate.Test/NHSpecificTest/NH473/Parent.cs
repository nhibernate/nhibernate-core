using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH473
{
	public class Parent
	{
		public Parent()
		{
			this.Children = new List<Child>();
		}
		public virtual int Id { get; set; }
		public virtual IList<Child> Children { get; set; }
	}
}
