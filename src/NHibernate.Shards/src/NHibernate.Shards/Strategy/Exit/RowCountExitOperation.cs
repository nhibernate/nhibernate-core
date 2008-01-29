using System.Collections;
using System.Collections.ObjectModel;
using NHibernate.Expressions;
using NHibernate.Shards.Util;

namespace NHibernate.Shards.Strategy.Exit
{
	public class RowCountExitOperation : IProjectionExitOperation
	{
		public RowCountExitOperation(IProjection projection)
		{
			Preconditions.CheckState(projection is RowCountProjection);
		}

		#region IProjectionExitOperation Members

		/// <summary>
		/// Return the collection with only one element, the result size.
		/// </summary>
		/// <param name="results"></param>
		/// <returns></returns>
		public IList Apply(IList results)
		{
			IList nonNullResults = ExitOperationUtils.GetNonNullList(results);

			ReadOnlyCollection<object> readOnlyCollection =
				new ReadOnlyCollection<object>(new object[] {nonNullResults.Count});

			return readOnlyCollection;
		}

		#endregion
	}
}