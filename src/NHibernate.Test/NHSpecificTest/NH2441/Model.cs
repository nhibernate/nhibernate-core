namespace NHibernate.Test.NHSpecificTest.NH2441
{
	public class Person
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual string Biography { get; set; }

		public Person()
		{
		}

		public Person(string name, string bio)
		{
			this.Name = name;
			this.Biography = bio;
		}
	}
}