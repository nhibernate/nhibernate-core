using System;

namespace NHibernate.Test.Hql
{
	public class Name
	{

		protected Name() { }

		public Name(string first, char initial, string last)
		{
			_first = first;
			_initial = initial;
			_last = last;
		}

		private string _first;
		public virtual string First
		{
			get { return _first; }
			set { _first = value; }
		}

		private char _initial;
		public virtual char Initial
		{
			get { return _initial; }
			set { _initial = value; }
		}

		private string _last;
		public virtual string Last
		{
			get { return _last; }
			set { _last = value; }
		}
	}
}
