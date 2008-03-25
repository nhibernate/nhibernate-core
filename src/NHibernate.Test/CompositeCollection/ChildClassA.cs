using System;

namespace NHibernate.Test.CompositeCollection
{
	/// <summary>
	/// Summary description for ChildClass.
	/// </summary>
	public class ChildClassA
	{
		private int _childID = 0;
		private BaseClassA _base = null;

		public BaseClassA Base
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