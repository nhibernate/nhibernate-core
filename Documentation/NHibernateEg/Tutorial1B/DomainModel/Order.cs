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
	/// Order.
	/// </summary>
	[NHMA.Class(Table="`Order`")]
	public class Order
	{
		private int _id = 0;
		private System.DateTime _date = System.DateTime.Now;
		private string _customer = string.Empty;
		private float _totalPrice = 0;

		[NHMA.Bag(Name="Details", Inverse=true, Lazy=true, Cascade=NHMA.CascadeStyle.AllDeleteOrphan)]
			[NHMA.Key(1, Column="`Order`")]
			[NHMA.OneToMany(2, ClassType=typeof(OrderDetail))]
		private System.Collections.IList _details = new System.Collections.ArrayList();


		/// <summary> Gets the unique identifier of this order. </summary>
		[NHMA.Id(Name="Id")]
			[NHMA.Generator(1, Class="native")]
		public virtual int Id
		{
			get { return _id; }
		}

		/// <summary> Gets when this order has been done. </summary>
		[NHMA.Property(Column="`Date`")]
		public virtual System.DateTime Date
		{
			get { return _date; }
		}

		/// <summary> Gets or sets the information about the customer who did this order. </summary>
		[NHMA.Property(NotNull=true)]
		public virtual string Customer
		{
			get { return _customer; }
			set { _customer = value; }
		}

		/// <summary> Gets the total price of this order. </summary>
		[NHMA.Property]
		public virtual float TotalPrice
		{
			get
			{
				try
				{
					// Hack to be sure that the collection is loaded (or lazy load it if possible)
					this.DetailsCount.ToString(); // This is done just to not reset the totalPrice if the lazy loading fails
				}
				catch { return _totalPrice; } // As the collection is lazy, the total price hasn't changed

				_totalPrice = 0;
				foreach(OrderDetail detail in _details)
					_totalPrice += detail.TotalPrice;

				return _totalPrice;
			}
		}


		/// <summary> Default Constructor. </summary>
		public Order()
		{
		}



		/// <summary> Gets the number of OrderDetail actually contained in this order. </summary>
		public virtual int DetailsCount
		{
			get
			{
				return this._details.Count;
			}
		}

		/// <summary> Adds an OrderDetail to this order. </summary>
		public virtual int Add(OrderDetail item)
		{
			if( item == null )
				throw new System.ArgumentNullException("item");
			if( this.Contains(item) )
				throw new System.ArgumentException("This item is already in the collection.", "item");

			int pos = this._details.Add(item);
			item.Order = this;
			return pos;
		}

		/// <summary> Removes all OrderDetails from this order. </summary>
		public virtual void ClearDetails()
		{
			this._details.Clear();
		}

		/// <summary> Determines whether this order contains the specified OrderDetail. </summary>
		public virtual bool Contains(OrderDetail item)
		{
			return this._details.Contains(item);
		}

		/// <summary> Determines whether this order contains an OrderDetail with the specified identifier. </summary>
		public virtual bool ContainsDetail(int id)
		{
			foreach(OrderDetail detail in this._details)
				if( detail.Id == id )
					return true;
			return false;
		}

		/// <summary> Gets the OrderDetail with the specified identifier. </summary>
		public virtual OrderDetail GetDetail(int id)
		{
			foreach(OrderDetail detail in this._details)
				if( detail.Id == id )
					return detail;
			throw new System.ArgumentException("There is no OrderDetail with the specified identifier.", "id");
		}

		/// <summary> Gets the OrderDetail at the specified index. </summary>
		public virtual OrderDetail GetDetailAt(int index)
		{
			return this._details[index] as OrderDetail;
		}

		/// <summary> Determines the index of a specific OrderDetail in this order. </summary>
		public virtual int IndexOf(OrderDetail item)
		{
			return this._details.IndexOf(item);
		}

		/// <summary> Inserts a OrderDetail to this order at the specified position. </summary>
		public virtual void Insert(int index, OrderDetail item)
		{
			if( item == null )
				throw new System.ArgumentNullException("item");
			if( this.Contains(item) )
				throw new System.ArgumentException("This item is already in the collection.", "item");

			this._details.Insert(index, item);
			item.Order = this;
		}

		/// <summary> Removes the specific OrderDetail from this order. </summary>
		public virtual void Remove(OrderDetail item)
		{
			this._details.Remove(item);
			item.Order = null;
			// TODO: To keep UnitsInStock accurate, we should do something like:
			// if(item.Id!=0 && item.Product!=null)
			// { item.Product.UnitsInStock += item.Quantity; Program.Shop.Save(item.Product); }
		}

		/// <summary> Removes the OrderDetail at the specified index. </summary>
		public virtual void RemoveDetailAt(int index)
		{
			((OrderDetail)this._details[index]).Order = null;
			this._details.RemoveAt(index);
		}




		/// <summary> Make sure that this instance is in a valid state (and can eventually be persised). </summary>
		public virtual void Validate()
		{
			foreach(OrderDetail detail in _details)
				detail.Validate();

			if( this.TotalPrice == 0 )
				throw new System.InvalidOperationException("You must order something.");
		}
	}
}
