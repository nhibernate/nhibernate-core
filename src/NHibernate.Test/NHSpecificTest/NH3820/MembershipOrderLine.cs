namespace NHibernate.Test.NHSpecificTest.NH3820
{
    public class MembershipOrderLine
    {
        public MembershipOrderLine(MembershipOrder order, string productCode) :this()
        {
            Order = order;
            ProductCode = productCode;
        }

        public MembershipOrderLine()
        {
        }
        public virtual int Id { get; set; }

        public virtual MembershipOrder Order { get; set; }

        public virtual string ProductCode { get; set; }
    }
}
