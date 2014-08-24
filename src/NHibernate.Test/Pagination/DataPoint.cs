namespace NHibernate.Test.Pagination
{
	public class DataPoint
	{
		public virtual long Id { get; set; }
		public virtual double X { get; set; }
		public virtual double Y { get; set; }
		public virtual string Description { get; set; }
	}
}