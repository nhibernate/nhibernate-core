using System;
using Iesi.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH830
{
	public class Cat
	{
		private int id;
		private ISet<Cat> children = new HashedSet<Cat>();
		private ISet<Cat> parents = new HashedSet<Cat>();

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