using System;
using System.Collections;

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
		private Iesi.Collections.ISet _manies;
		private int _x;
		
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

		public Iesi.Collections.ISet Manies
		{
			get { return _manies; }
			set { _manies = value; }
		}

		public int X
		{
			get { return _x; }
			set { _x = value; }
		}

	}
}
