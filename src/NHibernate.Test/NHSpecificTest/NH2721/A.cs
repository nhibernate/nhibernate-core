using System;
using System.Collections;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH2721
{
	public class A
	{
		private IList<B> _bs = new List<B>();

		public A()
		{
		}

		public A(string name)
		{
			Name = name;
		}

	    public int? Id { get; set; }
	    public string Name { get; set; }

		public IList<B> Bs
		{
			get { return _bs; }
			set { _bs = value; }
		}
	}
}
