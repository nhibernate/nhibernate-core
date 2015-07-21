namespace NHibernate.Test.NHSpecificTest.NH1788
{
	public class Person
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual byte[] Version { get; set; }
	}
}