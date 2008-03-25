using System;
using Iesi.Collections;

namespace NHibernate.Test.NHSpecificTest.NH830
{
	public class Cat
	{
		private int id;
		private ISet children = new HashedSet();
		private ISet parents = new HashedSet();

		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		public ISet Children
		{
			get { return children; }
			set { children = value; }
		}

		public ISet Parents
		{
			get { return parents; }
			set { parents = value; }
		}
	}
}