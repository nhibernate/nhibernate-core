namespace NHibernate.Test.Hql.Ast
{
	public class Vehicle
	{
		private long id;
		private string vin;
		private string owner;

		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual string Vin
		{
			get { return vin; }
			set { vin = value; }
		}

		public virtual string Owner
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