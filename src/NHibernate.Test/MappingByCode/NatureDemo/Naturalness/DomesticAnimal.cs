namespace NHibernate.Test.MappingByCode.NatureDemo.Naturalness
{
	public class DomesticAnimal: Mammal
	{
		public virtual Human Owner { get; set; }
	}

	public class Cat : DomesticAnimal { }
	public class Dog : DomesticAnimal { }
}