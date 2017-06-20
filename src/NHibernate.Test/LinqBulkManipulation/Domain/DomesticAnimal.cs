namespace NHibernate.Test.LinqBulkManipulation.Domain
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