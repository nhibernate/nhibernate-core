using System;

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

		private void Level1() { Level2(); }
		private void Level2() { throw new ArgumentException("thrown from Level2"); }

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

		public void ThrowDeepException()
		{
			Level1();
		}

		#endregion
	}
}
