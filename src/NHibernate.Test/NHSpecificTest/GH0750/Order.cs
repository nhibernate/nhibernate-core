using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.GH0750
{
	public class Order
	{
		private readonly ISet<OrderLine> _orderLines;
		private DateTime? _orderDate;
		private int _orderId;

		public Order()
		{
			_orderLines = new HashSet<OrderLine>();
		}

		public virtual int OrderId
		{
			get { return _orderId; }
			set { _orderId = value; }
		}

		public virtual DateTime? OrderDate
		{
			get { return _orderDate; }
			set { _orderDate = value; }
		}

		public virtual ISet<OrderLine> OrderLines
		{
			get { return _orderLines; }
		}

		public virtual void AddOrderLine(OrderLine orderLine)
		{
			if (!_orderLines.Contains(orderLine))
			{
				orderLine.Order = this;
				_orderLines.Add(orderLine);
			}
		}

		public virtual void RemoveOrderLine(OrderLine orderLine)
		{
			if (_orderLines.Contains(orderLine))
			{
				_orderLines.Remove(orderLine);
				orderLine.Order = null;
			}
		}
	}
}
