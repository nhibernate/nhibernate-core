namespace NHibernate.Test.NHSpecificTest.GH2608
{
	public class Person
	{
		public virtual long Id { get; set; }
		public virtual string Name { get; set; }
		public virtual PersonalDetails Details { get; set; }
	}
}
