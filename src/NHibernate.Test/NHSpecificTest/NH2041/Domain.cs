namespace NHibernate.Test.NHSpecificTest.NH2041
{
	public class MyClass
	{
		public Coordinates Location { get; set; }
	}

	public class Coordinates
	{
		public decimal Latitude { get; set; }
		public decimal Longitude { get; set; }
	}
}