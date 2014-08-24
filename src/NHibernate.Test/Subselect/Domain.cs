namespace NHibernate.Test.Subselect
{
	public class Human
	{
		public virtual int Id { get; set; }
		public virtual char Sex { get; set; }
		public virtual string Name { get; set; }
		public virtual string Address { get; set; }
	}

	public class Alien
	{
		public virtual int Id { get; set; }
		public virtual string Identity { get; set; }
		public virtual string Planet { get; set; }
		public virtual string Species { get; set; }
	}

	public class Being
	{
		public virtual int Id { get; set; }
		public virtual string Identity { get; set; }
		public virtual string Location { get; set; }
		public virtual string Species { get; set; }
	}
}