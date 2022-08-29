namespace NHibernate.Test.LinqBulkManipulation.Domain
{
	public class SimpleClassWithComponent
	{
		public virtual string Description { get; set; }
		public virtual long LongValue { get; set; }
		public virtual int IntValue { get; set; }
		public virtual Name Name { get; set; }
	}
}
