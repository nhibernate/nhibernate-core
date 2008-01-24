using System;
using System.Collections;
using NHibernate.Expressions;
using NHibernate.Shards.Util;

namespace NHibernate.Shards.Strategy.Exit
{
	public class OrderExitOperation : IExitOperation
	{
		private readonly Order order;

		private readonly string propertyName;

		public OrderExitOperation(Order order, string propertyName)
		{
			Preconditions.CheckState(order.ToString().EndsWith("asc") ||
			                         order.ToString().EndsWith("desc"));


			this.order = order;
			this.propertyName = GetSortingProperty(order);
		}

		#region IExitOperation Members

		public IList Apply(IList results)
		{
			//IList nonNullList = ExitOperationUtils.GetNonNullList(results);
			//IComparer comparer = new Comparer()
			throw new NotImplementedException();
		}

		#endregion

		private static string GetSortingProperty(Order order)
		{
			/**
			 * This method relies on the format that Order is using:
			 * propertyName + ' ' + (ascending?"asc":"desc")
			 */
			string str = order.ToString();
			return str.Substring(0, str.IndexOf(' '));
		}
	}
}