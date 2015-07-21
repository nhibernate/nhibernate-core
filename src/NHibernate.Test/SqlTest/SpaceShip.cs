using System;

namespace NHibernate.Test.SqlTest
{
	public class SpaceShip
	{
		private int id;
		private string name;
		private string model;
		private double speed;
		private Dimension dimensions;

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

		public virtual string Model
		{
			get { return model; }
			set { model = value; }
		}

		public virtual double Speed
		{
			get { return speed; }
			set { speed = value; }
		}

		public virtual Dimension Dimensions
		{
			get { return dimensions; }
			set { dimensions = value; }
		}
	}
}