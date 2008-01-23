using System.Collections;

namespace NHibernate.Shards.Strategy.Exit
{
	public class CountExitOperation : IProjectionExitOperation
	{
		#region IProjectionExitOperation Members

		public IList Apply(IList results)
		{
			throw new System.NotImplementedException();
		}

		#endregion
	}
}