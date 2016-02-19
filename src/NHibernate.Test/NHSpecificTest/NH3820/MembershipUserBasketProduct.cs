namespace NHibernate.Test.NHSpecificTest.NH3820
{
    #region using

    using System;

    #endregion

    public class MembershipUserBasketProduct
    {
        public MembershipUserBasketProduct()
        {
        }

        public MembershipUserBasketProduct(MembershipUserBasket basket, DateTime createDateTime, Product product)
            : this()
        {
            this.Basket = basket;
            this.CreateDateTime = createDateTime;
            this.Product = product;
        }

        public virtual MembershipUserBasket Basket { get; protected set; }

        public virtual DateTime CreateDateTime { get; protected set; }

        public virtual int Id { get; protected set; }

        public virtual Product Product { get; protected set; }
    }
}
