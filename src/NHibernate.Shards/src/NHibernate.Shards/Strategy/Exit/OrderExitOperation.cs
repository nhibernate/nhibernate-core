using System.Collections;
using System.Collections.Generic;
using NHibernate.Expressions;
using NHibernate.Shards.Util;

namespace NHibernate.Shards.Strategy.Exit
{
	public class OrderExitOperation : IExitOperation
	{
		//private readonly Order order;
		private readonly string propertyName;
		private bool IsAsc;
		private bool IsDesc;

		public OrderExitOperation(Order order)
		{
			IsAsc = order.ToString().EndsWith("asc");
			IsDesc = order.ToString().EndsWith("desc");

			Preconditions.CheckState(IsAsc || IsDesc);

			//this.order = order;
			propertyName = GetSortingProperty(order);
		}

		#region IExitOperation Members

		public IList Apply(IList results)
		{
			IList nonNullList = ExitOperationUtils.GetNonNullList(results);
			IComparer<object> comparer = new ExitOperationUtils.PropertyComparer(propertyName);


			List<object> sortedList = new List<object>((IEnumerable<object>) nonNullList);

			sortedList.Sort(comparer);

			if (IsDesc)
				sortedList.Reverse();

			return sortedList;
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