namespace NHibernate.Stat
{
	/// <summary> Statistics SPI for the NHibernate core </summary>
	public interface IStatisticsImplementor
	{
		void OpenSession();
		void CloseSession();
		void Flush();
		void Connect();
		void LoadEntity(string entityName);
		void FetchEntity(string entityName);
		void UpdateEntity(string entityName);
		void InsertEntity(string entityName);
		void DeleteEntity(string entityName);
		void LoadCollection(string role);
		void FetchCollection(string role);
		void UpdateCollection(string role);
		void RecreateCollection(string role);
		void RemoveCollection(string role);
		void SecondLevelCachePut(string regionName);
		void SecondLevelCacheHit(string regionName);
		void SecondLevelCacheMiss(string regionName);
		void QueryExecuted(string hql, int rows, long time);
		void QueryCacheHit(string hql, string regionName);
		void QueryCacheMiss(string hql, string regionName);
		void QueryCachePut(string hql, string regionName);
		void EndTransaction(bool success);
		void CloseStatement();
		void PrepareStatement();
		void OptimisticFailure(string entityName);
	}
}