using System;
using NHibernate.Test.ProxyInterface;

namespace NHibernate.Test.ProxyInterface
{
	/// <summary>
	/// Summary description for CastleProxyImpl.
	/// </summary>
	[Serializable]
	public class CastleProxyImpl : CastleProxy
	{
		private int _id;
		private string _name;

		#region CastleProxy Members

		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}
		
		#endregion

	}
}
