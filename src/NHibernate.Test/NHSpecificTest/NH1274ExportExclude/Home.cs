namespace NHibernate.Test.NHSpecificTest.NH1274ExportExclude
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

	public class Home_Update : Home { public Home_Update() { } public Home_Update(string city, int zip) : base(city, zip) { } }

	public class Home_Export : Home { public Home_Export() { } public Home_Export(string city, int zip) : base(city, zip) { } }

	public class Home_Validate : Home { public Home_Validate() { } public Home_Validate(string city, int zip) : base(city, zip) { } }

	public class Home_Drop : Home { public Home_Drop() { } public Home_Drop(string city, int zip) : base(city, zip) { } }

	public class Home_None : Home {public Home_None() { }  public Home_None(string city, int zip) : base(city, zip) { } }

	public class Home_All : Home {public Home_All() { }  public Home_All(string city, int zip) : base(city, zip) { } }

}