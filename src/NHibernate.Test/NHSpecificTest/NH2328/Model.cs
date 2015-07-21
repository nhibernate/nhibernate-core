namespace NHibernate.Test.NHSpecificTest.NH2328
{
	public class ToyBox
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual IShape Shape { get; set; }
	}

	public interface IShape
	{
	}

	public class Circle : IShape
	{
		public virtual int Id { get; set; }
	}

	public class Square : IShape
	{
		public virtual int Id { get; set; }
	}
}