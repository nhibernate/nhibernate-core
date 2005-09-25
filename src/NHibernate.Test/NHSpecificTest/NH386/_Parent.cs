using System;
using System.Collections;

using Iesi.Collections;

namespace NHibernate.Test.NHSpecificTest.NH386
{
	public class _Parent
	{
		private int _id;
		private ISet _children;

		public int _Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public ISet _Children
		{
			get { return _children; }
			set { _children = value; }
		}
	}
}
