using System;
using System.Collections;

namespace NHibernate.DomainModel
{
	public class S
	{
		private int    _count;
		private string _address;

		public S( int count, string address )
		{
			_count = count;
			_address = address;
		}

		public int Count
		{
			get { return _count; }
			set { _count = value; }
		}

		public string Address
		{
			get { return _address; }
			set { _address = value; }
		}

	}
}
