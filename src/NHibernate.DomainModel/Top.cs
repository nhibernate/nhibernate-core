using System;

namespace NHibernate.DomainModel
{
	[Serializable]
	public class Top
	{
		private long id;
		private string name;
		private string address;
		private int count;
		// initializing to a value that is ms independent because of MsSql millisecond
		// accurracy issues
		private DateTime date = new DateTime(2004, 01, 01, 12, 00, 00, 00);
		private Top other;
#pragma warning disable 169
		private Top top;
#pragma warning restore 169

		public Top(int c)
		{
			count = c;
		}

		public Top()
		{
		}

		public void Init()
		{
			name = "Someone with along name";
			address = "1234 some street, some city, victoria, 3000, austaya";
			count = 69;
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

		public Top Other
		{
			get { return other; }
			set { other = value; }
		}

		public long Id
		{
			get { return id; }
			set { id = value; }
		}
	}
}