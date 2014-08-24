namespace NHibernate.Test.NHSpecificTest.NH1495
{
	public interface IPerson
	{
		object Id { get;}
		string Name { get; set; }
	}

	public class Person : IPerson
	{
		public object Id { get; private set; }

		public string Name { get; set; }
	}
}