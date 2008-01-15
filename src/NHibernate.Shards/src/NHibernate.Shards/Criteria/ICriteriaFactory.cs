using NHibernate;

namespace NHibernate.Shards.Criteria
{
	/// <summary>
	/// Factory that knows how to create a <see cref="ICriteria"/> for a given <see cref="ISession"/>
	/// </summary>
	public interface ICriteriaFactory
	{
		/// <summary>
		/// Create a <see cref="ICriteria"/> for a given <see cref="ISession"/>
		/// </summary>
		/// <param name="session">the <see cref="ISession"/>  to be used when creating the <see cref="ICriteria"/></param>
		/// <returns></returns>
		ICriteria CreateCriteria(ISession session);
	}
}