using System;

namespace NHibernate.DomainModel
{

	[Serializable]
	public class Fee
	{
		public Fee _fee;
		public Fee _anotherFee;
		public String _fi;
		public String _key;
		public System.Collections.IDictionary _fees;
		private Qux _qux;
		private FooComponent _compon;
		private int _count;
	
		public Fee() 
		{
		}
	
		public Fee fee
		{
			get
			{
				return _fee;
			}
			set
			{
	
				this._fee = value;
			}
		}	
		public string fi
		{
			get

			{
				return _fi;
			}
			set
			{
				this._fi = value;
			}
		}
		public string key
		{
			get
			{
				return _key;
			}
			set
			{
				this._key = value;
			}
		}
	
		public System.Collections.IDictionary fees
		{
			get
			{
				return _fees;
			}
			set
			{
				this._fees = value;
			}
		}
	
		public Fee anotherFee
		{
			get
			{
				return _anotherFee;
			}
			set
			{
				this._anotherFee = value;
			}
		}
	
		public Qux qux
		{
			get
			{
				return _qux;
			}
			set
			{
				this._qux = value;
			}
		}
	
		public FooComponent compon
		{
			get
			{
				return _compon;
			}
			set
			{
				this._compon = value;
			}
		}	
		public int count
		{
			get
			{
				return _count;
			}
			set
			{
				_count = value;
			}
		}
	}
}