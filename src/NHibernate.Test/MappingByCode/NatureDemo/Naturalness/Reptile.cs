namespace NHibernate.Test.MappingByCode.NatureDemo.Naturalness
{
	public class Reptile : Animal
	{
		public virtual float BodyTemperature { get; set; }
	}

	public class Lizard : Reptile {}
}