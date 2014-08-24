using System;
using System.Collections.Generic;

namespace NHibernate.DomainModel
{
	/// <summary>
	/// Summary description for One.
	/// </summary>
	public class One
	{
		private long _key;
		private string _value;
		// <set> mapping
		private ISet<Many> _manies;
		private int _x;
		private int _v;

		public long Key
		{
			get { return _key; }
			set { _key = value; }
		}

		public string Value
		{
			get { return _value; }
			set { _value = value; }
		}

		public ISet<Many> Manies
		{
			get { return _manies; }
			set { _manies = value; }
		}

		public int X
		{
			get { return _x; }
			set { _x = value; }
		}

		public int V
		{
			get { return _v; }
			set { _v = value; }
		}
	}
}