using System.Collections;
using System.Collections.Generic;
using NHibernate.Engine;

namespace NHibernate.Shards.Strategy.Exit
{
	/// <summary>
	/// Classes that implement this interface are designed to manage the results
	/// of a incomplete execution of a query/critieria. For example, with averages
	/// the result of each query/critieria should be a list objects on which to
	/// calculate the average, rather than the avgerages on each shard. Or the
	/// the sum of maxResults(200) should be the sum of only 200 results, not the
	/// sum of the sums of 200 results per shard.
	/// </summary>
	public interface IExitOperationsCollector
	{
		IList Apply(IList result);

		void SetSessionFactory(ISessionFactoryImplementor sessionFactoryImplementor);
	}
}