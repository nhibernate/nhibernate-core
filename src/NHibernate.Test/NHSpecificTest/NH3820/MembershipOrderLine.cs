namespace NHibernate.Test.NHSpecificTest.NH3820
{
    public class MembershipOrderLine
    {
        public MembershipOrderLine(Product product, int quantity)
            : this()
        {
            this.Product = product;
            this.Quantity = quantity;
        }

        public MembershipOrderLine()
        {
        }

        public virtual int Id { get; protected set; }

        public virtual MembershipOrder Order { get; protected set; }

        public virtual Product Product { get; protected set; }

        public virtual int Quantity { get; protected set; }

        public virtual MembershipOrderLine SetOrder(MembershipOrder order)
        {
            this.Order = order;
            return this;
        }
    }
}
