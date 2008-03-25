using System;
using System.Collections;

namespace NHibernate.Test.NHSpecificTest.NH704
{
	public class Cat
	{
		private int id;
		private IList children = new ArrayList();
		//Note: I initialized the collection here, if removed, Lock will work.

		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		public IList Children
		{
			get { return children; }
			set { children = value; }
		}
	}
}