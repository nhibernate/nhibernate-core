using System.Collections;

namespace NHibernate.Shards.Strategy.Exit
{
	public class ShardedAvgExitOperation : IProjectionExitOperation
	{
		#region IProjectionExitOperation Members

		public IList Apply(IList results)
		{
			throw new System.NotImplementedException();
		}

		#endregion
	}
}