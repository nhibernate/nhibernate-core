namespace NHibernate.Test.BulkManipulation
{
	public class Vehicle
	{
		private long id;
		private string vin;
		private string owner;

		public long Id
		{
			get { return id; }
			set { id = value; }
		}

		public string Vin
		{
			get { return vin; }
			set { vin = value; }
		}

		public string Owner
		{
			get { return owner; }
			set { owner = value; }
		}
	}

	public class Car : Vehicle
	{
	}

	public class Truck : Vehicle
	{
	}

	public class Pickup : Truck
	{
	}

	public class SUV : Truck
	{
	}
}