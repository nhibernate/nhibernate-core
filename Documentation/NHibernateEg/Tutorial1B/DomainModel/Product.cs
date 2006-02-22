/******************************************************************************\
 *
 * NHibernateEg.Tutorial1B
 * Copyright © 2006, Pierre Henri Kuaté. All rights reserved.
 *
 * This product is under the terms of the GNU Lesser General Public License.
 * Read the file "license.txt" for more details.
 *
\*/

using NHMA = NHibernate.Mapping.Attributes;

namespace NHibernateEg.Tutorial1B.DomainModel
{
	/// <summary>
	/// Available product (that you can order).
	/// </summary>
	[NHMA.Class(Lazy=true)]
	public class Product
	{
		private System.Guid _id = System.Guid.Empty;
		private int _version = 0;
		private string _name;
		private float _unitPrice;
		private int _unitsInStock;


		/// <summary> Gets the unique identifier of this product. </summary>
		[NHMA.Id(Name="Id", Access="nosetter.camelcase-underscore")]
			[NHMA.Generator(1, Class="guid")]
		public virtual System.Guid Id
		{
			get { return _id; }
		}

		/// <summary> Gets the version (used for optimistic locking). </summary>
		[NHMA.Version]
		public virtual int Version
		{
			get { return _version; }
		}

		/// <summary> Gets or sets the name of this product. </summary>
		[NHMA.Property(NotNull=true)]
		public virtual string Name
		{
			get { return _name; }
			set
			{
				if( value==null || value==string.Empty )
					throw new System.ArgumentNullException("value", "The product's name cannot be empty.");

				_name = value;
			}
		}

		/// <summary> Gets or sets the price of one unit of this product. </summary>
		[NHMA.Property]
		public virtual float UnitPrice
		{
			get { return _unitPrice; }
			set
			{
				if( value <= 0 )
					throw new System.ArgumentException("UnitPrice must be strictly positive", "value");

				_unitPrice = value;
			}
		}

		/// <summary> Gets or sets the number of units of this product available in the data source. </summary>
		[NHMA.Property]
		public virtual int UnitsInStock
		{
			get { return _unitsInStock; }
			set
			{
				if( value < 0 )
					throw new System.ArgumentException("UnitsInStock must be positive or null (are you trying to sell non-existent items?)", "value");

				_unitsInStock = value;
			}
		}


		/// <summary> Default Constructor. </summary>
		public Product()
		{
		}


		/// <summary> Constructor taking the name and the unit price of the product. </summary>
		public Product(string name, float unitPrice)
		{
			this.Name = name;
			this.UnitPrice = unitPrice;
			this.UnitsInStock = 0;
		}




		/// <summary> Make sure that this instance is in a valid state (and can eventually be persised). </summary>
		public virtual void Validate()
		{
			// Use the properties validations (simpler but not recommended)
			this.Name = this.Name;
			this.UnitPrice = this.UnitPrice;
			this.UnitsInStock = this.UnitsInStock;
		}
	}
}
