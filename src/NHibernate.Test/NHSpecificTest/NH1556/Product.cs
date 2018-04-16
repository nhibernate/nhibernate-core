using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH1556
{
   public class Product
   {
      protected Product()
      {
         productIdentifiers = new List<ProductIdentifier>();
      }

      public Product(string productName) : this()
      {
         this.productName = productName;
      }

      // Assigned by reflection
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
      private long id;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value
      public virtual long Id
      {
         get { return id; }
      }

      private string productName;
      public virtual string ProductName
      {
         get { return productName; }
      }

      private IList<ProductIdentifier> productIdentifiers;
      public virtual IList<ProductIdentifier> ProductIdentifiers
      {
         get { return productIdentifiers; }
      }

      public virtual void AddIdentifier(ProductIdentifier productIdentifier)
      {
         productIdentifiers.Add(productIdentifier);
      }
   }
}
