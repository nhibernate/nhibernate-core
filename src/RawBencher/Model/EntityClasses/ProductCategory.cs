﻿//------------------------------------------------------------------------------
// <auto-generated>This code was generated by LLBLGen Pro v4.2.</auto-generated>
//------------------------------------------------------------------------------
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NH.Bencher.EntityClasses
{
	/// <summary>Class which represents the entity 'ProductCategory'</summary>
	public partial class ProductCategory
	{
		#region Class Member Declarations
		private ISet<ProductSubcategory> _productSubcategories;
		private System.DateTime _modifiedDate;
		private System.String _name;
		private System.Int32 _productCategoryId;
		private System.Guid _rowguid;
		#endregion

		/// <summary>Initializes a new instance of the <see cref="ProductCategory"/> class.</summary>
		public ProductCategory() : base()
		{
			_productSubcategories = new HashSet<ProductSubcategory>();
			_productCategoryId = default(System.Int32);
			OnCreated();
		}

		/// <summary>Method called from the constructor</summary>
		partial void OnCreated();

		/// <summary>Returns a hash code for this instance.</summary>
		/// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. </returns>
		public override int GetHashCode()
		{
			int toReturn = base.GetHashCode();
			toReturn ^= this.ProductCategoryId.GetHashCode();
			return toReturn;
		}
	
		/// <summary>Determines whether the specified object is equal to this instance.</summary>
		/// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
		/// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.</returns>
		public override bool Equals(object obj)
		{
			if(obj == null) 
			{
				return false;
			}
			ProductCategory toCompareWith = obj as ProductCategory;
			return toCompareWith == null ? false : ((this.ProductCategoryId == toCompareWith.ProductCategoryId));
		}
		

		#region Class Property Declarations
		/// <summary>Gets or sets the ModifiedDate field. </summary>	
		public virtual System.DateTime ModifiedDate
		{ 
			get { return _modifiedDate; }
			set { _modifiedDate = value; }
		}

		/// <summary>Gets or sets the Name field. </summary>	
		public virtual System.String Name
		{ 
			get { return _name; }
			set { _name = value; }
		}

		/// <summary>Gets the ProductCategoryId field. </summary>	
		public virtual System.Int32 ProductCategoryId
		{ 
			get { return _productCategoryId; }
		}

		/// <summary>Gets or sets the Rowguid field. </summary>	
		public virtual System.Guid Rowguid
		{ 
			get { return _rowguid; }
			set { _rowguid = value; }
		}

		/// <summary>Represents the navigator which is mapped onto the association 'ProductSubcategory.ProductCategory - ProductCategory.ProductSubcategories (m:1)'</summary>
		public virtual ISet<ProductSubcategory> ProductSubcategories
		{
			get { return _productSubcategories; }
			set { _productSubcategories = value; }
		}
		
		#endregion
	}
}
