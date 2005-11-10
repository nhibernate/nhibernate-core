/******************************************************************************\
 *
 * NHibernateEg.Tutorial1A
 * Copyright © 2005, Pierre Henri Kuaté. All rights reserved.
 *
 * This product is under the terms of the GNU Lesser General Public License.
 * Read the file "license.txt" for more details.
 *
\*/

namespace NHibernateEg.Tutorial1A
{
	/// <summary>
	/// Entity "Order".
	/// Contain the name of the product purchased, the delivered quantity and the total price.
	/// </summary>
	[NHibernate.Mapping.Attributes.Class(Table="SimpleOrder")]
	public class Order
	{
		private int _id = 0;
		private System.DateTime _date = System.DateTime.Now;
		private string _product;
		private int _quantity;
		private int _totalPrice;

		/// <summary> Gets the unique identifier of this order. </summary>
		[NHibernate.Mapping.Attributes.Id(Name="Id")]
			[NHibernate.Mapping.Attributes.Generator(1, Class="native")]
		public virtual int Id
		{
			get { return _id; }
		}

		/// <summary> Tells when the order has been registered. </summary>
		[NHibernate.Mapping.Attributes.Property]
		public virtual System.DateTime Date
		{
			get { return _date; }
		}

		/// <summary> Name of the ordered product. </summary>
		[NHibernate.Mapping.Attributes.Property(NotNull=true)]
		public virtual string Product
		{
			get { return _product; }
			set { _product = value; }
		}

		/// <summary> Number of items ordered. </summary>
		[NHibernate.Mapping.Attributes.Property]
		public virtual int Quantity
		{
			get { return _quantity; }
			set
			{
				if(value <= 0)
					throw new System.ArgumentException("Quantity must be strictly positive!");
				_quantity = value;
			}
		}

		/// <summary> Total price of the order. </summary>
		[NHibernate.Mapping.Attributes.Property]
		public virtual int TotalPrice
		{
			get { return _totalPrice; }
			set { _totalPrice = value; }
		}



		/// <summary> Default constructor. </summary>
		public Order()
		{
		}


		/// <summary> Constructor with all properties. </summary>
		public Order(string product, int unitPrice, int quantity)
		{
			this.Product = product;
			this.Quantity = quantity; // Don't use _quantity
			this.ComputeTotalPrice(unitPrice);
		}


		/// <summary> Compute the total price of the order using the unit price (and the quantity). </summary>
		public void ComputeTotalPrice(int unitPrice)
		{
			this.TotalPrice = unitPrice * this.Quantity;
		}


		/// <summary> Add 'n' hours to the date. </summary>
		public void ChangeTimeZone(int n)
		{
			this._date = this.Date.AddHours(n);
		}
	}
}
