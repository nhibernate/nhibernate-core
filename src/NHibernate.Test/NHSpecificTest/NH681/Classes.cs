using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH681
{
	public class Foo
	{
		private int id;

		private IList<Foo> children = new List<Foo>();

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual IList<Foo> Children
		{
			get { return children; }
			set { children = value; }
		}
	}
}
