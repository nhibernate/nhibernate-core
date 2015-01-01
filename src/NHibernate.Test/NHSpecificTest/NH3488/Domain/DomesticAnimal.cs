namespace NHibernate.Test.NHSpecificTest.NH3488.Domain
{
	public class DomesticAnimal: Mammal
	{
		private Human owner;

		public virtual Human Owner
		{
			get { return owner; }
			set { owner = value; }
		}
	}

	public class Cat : DomesticAnimal { }
	public class Dog : DomesticAnimal { }
}