using System.Collections;

namespace NHibernate.Shards.Strategy.Exit
{
	/// <summary>
	/// Performs post-processing on a result set that has had an average projection
	/// applied.
	///
	/// This may not yield the exact same result as you'd get if you ran the query
	/// on a single shard because there seems to be some platform-specific wiggle.
	/// Here's a specific example:
	/// On hsqldb, if you have a column of type DECIMAL(10, 4) and you ask for the
	/// average of the values in that column, you get the floor of the result.
	/// On MySQL, if you have a column of the same type, you get a result back with
	/// the expected precision.  So, um, just be careful.
	/// </summary>
	public class AvgResultsExitOperation : IExitOperation
	{
		#region IExitOperation Members

		public IList Apply(IList results)
		{
			throw new System.NotImplementedException();
		}

		#endregion
	}
}