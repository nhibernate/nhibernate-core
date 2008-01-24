using System;
using System.Collections;

namespace NHibernate.Shards.Strategy.Exit
{
	public class MaxResultsExitOperation : IExitOperation
	{
		private readonly int maxResult;

		#region IExitOperation Members

		public MaxResultsExitOperation(int maxResult)
		{
			this.maxResult = maxResult;
		}

		public IList Apply(IList results)
		{
			IList nonNullResults = ExitOperationUtils.GetNonNullList(results);
			return ExitOperationUtils.GetSubList(nonNullResults, 0, Math.Min(nonNullResults.Count, maxResult));
		}

		#endregion
	}
}