using System;

namespace NHibernate.Test.ListIndex
{
	public class B
	{
		private int _id;
		private int _aId;
		private int _listIndex;
		private string _name;

		public virtual int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public virtual int AId
		{
			get { return _aId; }
			set { _aId = value; }
		}

		public virtual int ListIndex
		{
			get { return _listIndex; }
			set { _listIndex = value; }
		}

		public virtual string Name
		{
			get { return _name; }
			set { _name = value; }
		}
	}
}