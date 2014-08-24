using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH830
{
	public class Cat
	{
		private int id;
		private ISet<Cat> children = new HashSet<Cat>();
		private ISet<Cat> parents = new HashSet<Cat>();

		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		public ISet<Cat> Children
		{
			get { return children; }
			set { children = value; }
		}

		public ISet<Cat> Parents
		{
			get { return parents; }
			set { parents = value; }
		}
	}
}