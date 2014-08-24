using System;
using System.Collections.Generic;

namespace NHibernate.DomainModel
{
	[Serializable]
	public class Fee
	{
		public Fee _fee;
		public Fee _anotherFee;
		public String _fi;
		public String _key;
		public ISet<string> _fees;
		private Qux _qux;
		private FooComponent _compon;
		private int _count;

		public Fee()
		{
		}

		public Fee TheFee
		{
			get { return _fee; }
			set { _fee = value; }
		}

		public string Fi
		{
			get { return _fi; }
			set { _fi = value; }
		}

		public string Key
		{
			get { return _key; }
			set { this._key = value; }
		}

		public ISet<string> Fees
		{
			get { return _fees; }
			set { _fees = value; }
		}

		public Fee AnotherFee
		{
			get { return _anotherFee; }
			set { _anotherFee = value; }
		}

		public Qux Qux
		{
			get { return _qux; }
			set { _qux = value; }
		}

		public FooComponent Compon
		{
			get { return _compon; }
			set { _compon = value; }
		}

		public int Count
		{
			get { return _count; }
			set { _count = value; }
		}
	}
}