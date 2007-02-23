using System;
using System.Collections;

namespace NHibernate.Test.CompositeCollection
{
	/// <summary>
	/// Summary description for ChildColl.
	/// </summary>
	public class ChildColl
	{
		private IList _values = new ArrayList();

		public IList Values
		{
			get { return _values; }
			set { _values = value; }
		}
	}
}