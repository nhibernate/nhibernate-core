namespace NHibernate.Shards.Query
{
	/// <summary>
	/// TODO: documentation
	/// </summary>
	public interface IQueryFactory
	{
		/// <summary>
		/// TODO: documentation
		/// </summary>
		/// <param name="session"></param>
		/// <returns></returns>
		IQuery createQuery(ISession session);
	}
}