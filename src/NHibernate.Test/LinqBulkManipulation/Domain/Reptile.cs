namespace NHibernate.Test.LinqBulkManipulation.Domain
{
	public class Reptile : Animal
	{
		private float bodyTemperature;
		public virtual float BodyTemperature
		{
			get { return bodyTemperature; }
			set { bodyTemperature = value; }
		}
	}

	public class Dragon : Animal
	{
		private float fireTemperature;
		public virtual float FireTemperature
		{
			get { return fireTemperature; }
			protected set { fireTemperature = value; }
		}

		public virtual void SetFireTemperature(float temperature)
		{
			fireTemperature = temperature;
		}
	}

	public class Lizard : Reptile { }
}
