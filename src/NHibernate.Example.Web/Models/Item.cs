namespace NHibernate.Example.Web.Models
{
	public class Item
	{
		public virtual int Id { get; set; }
		public virtual decimal Price { get; set; }
		public virtual string Description { get; set; }
	}
}
