namespace NHibernate.Test.LazyOneToOne
{
	public class Person
	{
		public virtual string Name { get; set; }
		public virtual Employee Employee { get; set; }
	}
}