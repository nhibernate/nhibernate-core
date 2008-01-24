using System.Collections;
using System.Runtime.CompilerServices;

namespace NHibernate.Shards.Strategy.Exit
{
	/// <summary>
	/// Threadsafe ExistStrategy that concatenates all the lists that are added.
	/// </summary>
	public class ConcatenateListsExitStrategy : IExitStrategy<IList>
	{
		private readonly IList result = new ArrayList();

		#region IExitStrategy<IList> Members

		/// <summary>
		/// Add the provided result and return whether or not the caller can halt
		/// processing.
		/// </summary>
		/// <param name="oneResult">The result to add</param>
		/// <param name="shard"></param>
		/// <returns>Whether or not the caller can halt processing</returns>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public bool AddResult(IList oneResult, IShard shard)
		{
			foreach(object item in oneResult)
			{
				result.Add(item);
			}
			return false;
		}

		public IList CompileResults(IExitOperationsCollector exitOperationsCollector)
		{
			return exitOperationsCollector.Apply(result);
		}

		#endregion
	}
}