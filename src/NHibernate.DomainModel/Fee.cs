using System;

namespace NHibernate.DomainModel
{

	[Serializable]
	public class Fee
	{
		public Fee _fee;
		public Fee anotherFee;
		public String fi;
		public String key;
		public System.Collections.IDictionary fees;
		private Qux qux;
		private FooComponent compon;
		private int count;
	
		public Fee() 
		{
		}
	
		public Fee fee
		{
			get
			{
				return fee;
			}
			set
			{
	
				this.fee = value;
			}
		}	
		public string Fi
		{
			get

			{
				return fi;
			}
			set
			{
				this.fi = value;
			}
		}
		public string Key
		{
			get
			{
				return key;
			}
			set
			{
				this.key = value;
			}
		}
	
		public System.Collections.IDictionary Fees
		{
			get
			{
				return fees;
			}
			set
			{
				this.fees = value;
			}
		}
	
		public Fee AnotherFee
		{
			get
			{
				return anotherFee;
			}
			set
			{
				this.anotherFee = value;
			}
		}
	
		public Qux Qux
		{
			get
			{
				return qux;
			}
			set
			{
				this.qux = value;
			}
		}
	
		public FooComponent Compon
		{
			get
			{
				return compon;
			}
			set
			{
				this.compon = value;
			}
		}	
		public int Count
		{
			get
			{
				return count;
			}
			set
			{
				count = value;
			}
		}
	}
}