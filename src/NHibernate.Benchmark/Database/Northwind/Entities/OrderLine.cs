namespace NHibernate.DomainModel.Northwind.Entities
{
    public class OrderLine : Entity<OrderLine>
    {
	    private Order _order;
        private Product _product;

        public OrderLine() : this(null, null)
        {
        }

        public OrderLine(Order order, Product product)
        {
            _order = order;
            _product = product;
        }

        public virtual Order Order
        {
            get => _order;
            set => _order = value;
        }

        public virtual Product Product
        {
            get => _product;
            set => _product = value;
        }

        public virtual decimal UnitPrice { get; set; }

        public virtual int Quantity { get; set; }

        public virtual decimal Discount { get; set; }
    }
}