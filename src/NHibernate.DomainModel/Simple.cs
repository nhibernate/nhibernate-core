using System;

namespace NHibernate.DomainModel 
{
	[Serializable]
	public class Simple 
	{
		private string name;
		private string address;
		private int count;
		// initializing to a value that is ms independent because of MsSql millisecond
		// accurracy issues
		private DateTime date = new DateTime( 2004, 01, 01, 12, 00, 00, 00 );
		private Simple other;
		private float pay;

		public Simple(int c) 
		{
			count=c;
		}
		public Simple() {}

		public void Init() 
		{
			name="Someone with along name";
			address="1234 some street, some city, victoria, 3000, austaya";
			count=69;
			date = DateTime.Now;
		}

		public string Name 
		{
			get { return name; }
			set { name = value; }
		}

		public string Address 
		{
			get { return address; }
			set { address = value; }
		}

		public int Count 
		{
			get { return count; }
			set { count = value; }
		}

		public DateTime Date 
		{
			get { return date; }
			set { date = value; }
		}

		public Simple Other 
		{
			get { return other; }
			set { other = value; }
		}

		public float Pay
		{
			get { return pay; }
			set { pay = value; }
		}
	}
}
