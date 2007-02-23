using System;

namespace NHibernate.DomainModel
{
	/// <summary>
	/// Summary description for Componentizable.
	/// </summary>
	public class Componentizable
	{
		private int _id;
		public string _nickName;
		private Component _component;

		public Componentizable()
		{
		}

		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public Component Component
		{
			get { return _component; }
			set { _component = value; }
		}

		public string NickName
		{
			get { return _nickName; }
			set { _nickName = value; }
		}
	}
}