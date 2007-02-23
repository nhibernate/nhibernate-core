using System;

namespace NHibernate.Test.NHSpecificTest.NH295
{
	[Serializable]
	public abstract class Party
	{
		public const int EMPTY_ID = -1;

		private int _id = EMPTY_ID;
		private string _name;

		internal Party()
		{
		}

		internal Party(string name)
		{
			_name = name;
		}

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
	}
}