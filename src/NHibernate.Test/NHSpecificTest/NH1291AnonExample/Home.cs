namespace NHibernate.Test.NHSpecificTest.NH1291AnonExample
{
	public class Home
	{
		private int id;
		private int zip;
		private string city;

		public Home()
		{

		}

		public Home(string city, int zip)
		{
			this.city = city;
			this.zip = zip;
		}

		virtual public int Id
		{
			get { return id; }
			set { id = value; }
		}

		virtual public string City
		{
			get { return city; }
			set { city = value; }
		}

		virtual public int Zip
		{
			get { return zip; }
			set { zip = value; }
		}
	}
}