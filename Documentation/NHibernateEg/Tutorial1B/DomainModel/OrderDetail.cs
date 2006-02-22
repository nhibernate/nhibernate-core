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
	/// Item of an Order.
	/// </summary>
	[NHMA.Class]
	public class OrderDetail
	{
		private long _id;
		private Order _order;
		private Product _product;
		private string _productName;
		private float _unitPrice;
		private int _quantity;
		private int _quantityToRemove;


		/// <summary> Gets the unique identifier of this order detail. </summary>
		[NHMA.Id(Name="Id")]
			[NHMA.Generator(1, Class="native")]
		public virtual long Id
		{
			get { return _id; }
			set { _id = value; }
		}

		/// <summary> Gets or sets the order this detail belongs to. </summary>
		[NHMA.ManyToOne(Column="`Order`", NotNull=true)]
		public virtual Order Order
		{
			get { return _order; }
			set { _order = value; }
		}

		/// <summary> Gets or sets the ordered product. </summary>
		[NHMA.ManyToOne(NotNull=false)]
		public virtual Product Product
		{
			get { return _product; }
			set
			{
				_product = value;

				if(_product != null)
				{
					_productName = _product.Name;
					_unitPrice = _product.UnitPrice;
				}
			}
		}

		/// <summary> Gets the name of the ordered product. </summary>
		[NHMA.Property(NotNull=true)]
		public virtual string ProductName
		{
			get { return _productName; }
		}

		/// <summary> Gets the unit price of the ordered product. </summary>
		[NHMA.Property]
		public virtual float UnitPrice
		{
			get { return _unitPrice; }
		}

		/// <summary> Gets or sets the number of units ordered. </summary>
		[NHMA.Property]
		public virtual int Quantity
		{
			get { return _quantity; }
			set
			{
				if( value <= 0 )
					throw new System.ArgumentException("Quantity must be strictly positive", "value");

				_quantityToRemove -= Quantity;
				_quantity = value;
				_quantityToRemove += Quantity;
			}
		}


		public virtual float TotalPrice
		{
			get { return UnitPrice * Quantity; }
		}


		/// <summary> Default Constructor. </summary>
		public OrderDetail()
		{
		}


		public OrderDetail(Product product, int quantity)
		{
			this.Product = product;
			this.Quantity = quantity;
		}




		/// <summary> Make sure that this instance is in a valid state (and can eventually be persised). </summary>
		public virtual void Validate()
		{
			if(this.Order==null || this.ProductName==null || this.ProductName==string.Empty)
				throw new System.InvalidOperationException("Set the order and the product.");

			if( this.TotalPrice == 0 )
				throw new System.InvalidOperationException("You can't order zero " + this.ProductName);

			if( this._quantityToRemove != 0 )
			{
				this.Product.UnitsInStock -= this._quantityToRemove;
				this._quantityToRemove = 0;
			}
		}
	}
}
