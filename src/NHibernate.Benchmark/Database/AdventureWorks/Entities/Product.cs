using System;
using System.Collections.Generic;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class Product
	{
		public virtual int Id { get; set; }
		public virtual string Class { get; set; }
		public virtual string Color { get; set; }
		public virtual int DaysToManufacture { get; set; }
		public virtual DateTime? DiscontinuedDate { get; set; }
		public virtual bool FinishedGoodsFlag { get; set; }
		public virtual decimal ListPrice { get; set; }
		public virtual bool MakeFlag { get; set; }
		public virtual DateTime ModifiedDate { get; set; }
		public virtual string Name { get; set; }
		public virtual string ProductLine { get; set; }
		public virtual string ProductNumber { get; set; }
		public virtual short ReorderPoint { get; set; }
		public virtual Guid RowGuid { get; set; }
		public virtual short SafetyStockLevel { get; set; }
		public virtual DateTime? SellEndDate { get; set; }
		public virtual DateTime SellStartDate { get; set; }
		public virtual string Size { get; set; }
		public virtual string SizeUnitMeasureCode { get; set; }
		public virtual decimal StandardCost { get; set; }
		public virtual string Style { get; set; }
		public virtual decimal? Weight { get; set; }

		public virtual ICollection<BillOfMaterials> BillOfMaterials { get; set; } = new HashSet<BillOfMaterials>();
		public virtual ICollection<BillOfMaterials> BillOfMaterialsNavigation { get; set; } = new HashSet<BillOfMaterials>();
		public virtual ICollection<ProductCostHistory> ProductCostHistory { get; set; } = new HashSet<ProductCostHistory>();
		public virtual ICollection<ProductDocument> ProductDocument { get; set; } = new HashSet<ProductDocument>();
		public virtual ICollection<ProductInventory> ProductInventory { get; set; } = new HashSet<ProductInventory>();
		public virtual ICollection<ProductListPriceHistory> ProductListPriceHistory { get; set; } = new HashSet<ProductListPriceHistory>();
		public virtual ICollection<ProductProductPhoto> ProductProductPhoto { get; set; } = new HashSet<ProductProductPhoto>();
		public virtual ICollection<ProductReview> ProductReview { get; set; } = new HashSet<ProductReview>();
		public virtual ICollection<ProductVendor> ProductVendor { get; set; } = new HashSet<ProductVendor>();
		public virtual ICollection<PurchaseOrderDetail> PurchaseOrderDetail { get; set; } = new HashSet<PurchaseOrderDetail>();
		public virtual ICollection<ShoppingCartItem> ShoppingCartItem { get; set; } = new HashSet<ShoppingCartItem>();
		public virtual ICollection<SpecialOfferProduct> SpecialOfferProduct { get; set; } = new HashSet<SpecialOfferProduct>();
		public virtual ICollection<TransactionHistory> TransactionHistory { get; set; } = new HashSet<TransactionHistory>();
		public virtual ICollection<WorkOrder> WorkOrder { get; set; } = new HashSet<WorkOrder>();
		public virtual ProductModel ProductModel { get; set; }
		public virtual ProductSubcategory ProductSubcategory { get; set; }
		public virtual UnitMeasure SizeUnitMeasureCodeNavigation { get; set; }
		public virtual UnitMeasure WeightUnitMeasureCodeNavigation { get; set; }
	}
}
