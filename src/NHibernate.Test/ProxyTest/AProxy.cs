using System;

namespace NHibernate.Test.ProxyTest
{
	/// <summary>
	/// Summary description for AProxy.
	/// </summary>
	public class AProxy
	{
		int _id;
		string _name;

		public AProxy()
		{
		}

		public virtual int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public virtual string Name
		{
			get { return _name; }
			set { _name = value; }
		}

	}
}
