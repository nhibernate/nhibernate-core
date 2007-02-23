using System;

namespace NHibernate.Test.CompositeCollection
{
	/// <summary>
	/// Summary description for BaseClass.
	/// </summary>
	public class BaseClassB
	{
		private int _baseID = 0;
		private ChildColl _children = new ChildColl();

		public int BaseID
		{
			get { return _baseID; }
			set { _baseID = value; }
		}

		public ChildColl Children
		{
			get { return _children; }
			set { _children = value; }
		}
	}
}