using System;

namespace NHibernate.Test.CompositeCollection
{
	/// <summary>
	/// Summary description for ChildClassB.
	/// </summary>
	public class ChildClassB
	{
		private int _childID = 0;
		private BaseClassB _base = null;

		public BaseClassB Base
		{
			get { return _base; }
			set { _base = value; }
		}

		public int ChildID
		{
			get { return _childID; }
			set { _childID = value; }
		}
	}
}