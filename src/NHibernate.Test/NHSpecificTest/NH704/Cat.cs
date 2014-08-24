using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH704
{
	public class Cat
	{
		private int id;
		private IList<Cat> children = new List<Cat>();
		//Note: I initialized the collection here, if removed, Lock will work.

		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		public IList<Cat> Children
		{
			get { return children; }
			set { children = value; }
		}
	}
}