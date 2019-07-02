using System;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class ShoppingCartItem
	{
		public virtual int Id { get; set; }
		public virtual DateTime DateCreated { get; set; }
		public virtual DateTime ModifiedDate { get; set; }
		public virtual int Quantity { get; set; }
		public virtual string ShoppingCartId { get; set; }
		public virtual Product Product { get; set; }
	}
}
