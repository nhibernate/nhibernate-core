namespace NHibernate.Test.NHSpecificTest.NH3820
{
    #region using

    using System.Collections.Generic;

    #endregion

    public class MembershipOrder
    {
        public MembershipOrder(MembershipUser user, MembershipUserBasket basket)
            : this()
        {
            this.User = user;
            this.Basket = basket;
            this.Basket.SetOrder(this);
        }

        public MembershipOrder()
        {
            this.OrderLines = new List<MembershipOrderLine>();
        }

        public virtual MembershipUserBasket Basket { get; protected set; }

        public virtual int Id { get; protected set; }

        public virtual ICollection<MembershipOrderLine> OrderLines { get; protected set; }

        public virtual MembershipUser User { get; protected set; }

        public virtual MembershipOrder AddOrderLine(MembershipOrderLine line)
        {
            line.SetOrder(this);
            this.OrderLines.Add(line);
            return this;
        }
    }
}
