namespace NHibernate.Test.NHSpecificTest.NH3820
{
    #region using

    using System;
    using System.Collections.Generic;

    #endregion

    public class MembershipUserBasket
    {
        public MembershipUserBasket(Guid basketGuid, DateTime createDateTime, MembershipUser user)
            : this()
        {
            this.BasketGuid = basketGuid;
            this.CreateDateTime = createDateTime;
            this.User = user;
        }

        public MembershipUserBasket()
        {
            this.BasketProducts = new List<MembershipUserBasketProduct>();
        }

        public virtual Guid BasketGuid { get; protected set; }

        public virtual ICollection<MembershipUserBasketProduct> BasketProducts { get; protected set; }

        public virtual DateTime CreateDateTime { get; protected set; }

        public virtual int Id { get; protected set; }

        public virtual MembershipOrder Order { get; protected set; }

        public virtual MembershipUser User { get; protected set; }

        public virtual MembershipUserBasket SetOrder(MembershipOrder order)
        {
            this.Order = order;
            return this;
        }

        public virtual MembershipUserBasket AddProduct(MembershipUserBasketProduct basketProduct)
        {
            this.BasketProducts.Add(basketProduct);
            return this;
        }
    }
}
