namespace NHibernate.Test.NHSpecificTest.NH1556
{
   public class ProductIdentifier
   {
      protected ProductIdentifier()
      {
      }

      public ProductIdentifier(string identifierField, Product product)
      {
         this.identifierField = identifierField;
         this.product = product;
         this.product.AddIdentifier(this);
      }

      private long id;
      public virtual long Id
      {
         get { return id; }
      }

      private string identifierField;
      public virtual string IdentifierField
      {
         get { return identifierField; }
      }

      private Product product;
      public virtual Product Product
      {
         get { return product; }
      }
   }
}
