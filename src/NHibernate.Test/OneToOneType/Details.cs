namespace NHibernate.Test.OneToOneType
{
	public class Details
	{
		public virtual int Id { get; protected set; }
		public virtual Owner Owner { get; protected internal set; }
		public virtual string Data { get; set; }
	}
}
