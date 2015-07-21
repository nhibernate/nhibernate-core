using System;

namespace NHibernate.Test.Criteria
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

	public class Lizard : Reptile
	{
	}
}