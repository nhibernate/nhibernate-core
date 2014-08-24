using System;

namespace NHibernate.Test.NHSpecificTest.NH1033
{
	public class Reptile : Animal
	{

		public virtual float BodyTemperature
		{
			get;
			set;
		}
	}
}