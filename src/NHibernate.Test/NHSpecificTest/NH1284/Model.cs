namespace NHibernate.Test.NHSpecificTest.NH1284
{
	public class Person
	{
		private Address? _address;
		private string _name;
		protected Person() {}

		public Person(string name)
		{
			_name = name;
		}

		public virtual string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public virtual Address? Address
		{
			get { return _address; }
			set { _address = value; }
		}
	}

	public struct Address
	{
		private string _city;
		private int _gmtOffset;
		private string _street;

		public string Street
		{
			get { return _street; }
			set { _street = value; }
		}

		public string City
		{
			get { return _city; }
			set { _city = value; }
		}

		public int GmtOffset
		{
			get { return _gmtOffset; }
			set { _gmtOffset = value; }
		}
	}
}