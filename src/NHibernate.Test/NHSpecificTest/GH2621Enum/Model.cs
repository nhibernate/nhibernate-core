namespace NHibernate.Test.NHSpecificTest.GH2621Enum
{
	public abstract class ClassWithString
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual string Kind { get; set; }
	}

	public enum Kind 
	{
		SomeKind,
		SomeOtherKind
	}
}
