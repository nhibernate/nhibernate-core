using System;

namespace NHibernate.Test.Hql
{
	public class Human: Animal
	{
		private Name _name;
		public virtual Name Name
		{
			get { return _name; }
			set { _name = value; }
		}

		private String _nickName;
		public virtual String NickName
		{
			get { return _nickName; }
			set { _nickName = value; }
		}

		private DateTime _birthdate;
		public virtual DateTime Birthdate
		{
			get { return _birthdate; }
			set { _birthdate = value; }
		}

	}
}
