namespace NHibernate.Test.NHSpecificTest.NH2341
{
	public abstract class AbstractBA
	{
		public virtual int Id { get; set; }
	}
	public class ConcreteBA : AbstractBA
	{
	}
	public class ConcreteA : ConcreteBA
	{
	}
	public class ConcreteB : ConcreteBA
	{
	}
}