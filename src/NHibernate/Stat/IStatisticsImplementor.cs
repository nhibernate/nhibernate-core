using System;
namespace NHibernate.Stat
{
	/// <summary> Statistics SPI for the NHibernate core </summary>
	public interface IStatisticsImplementor
	{
		void OpenSession();
		void CloseSession();
		void Flush();
		void Connect();
		void LoadEntity(string entityName, TimeSpan time);
		void FetchEntity(string entityName, TimeSpan time);
		void UpdateEntity(string entityName, TimeSpan time);
		void InsertEntity(string entityName, TimeSpan time);
		void DeleteEntity(string entityName, TimeSpan time);
		void LoadCollection(string role, TimeSpan time);
		void FetchCollection(string role, TimeSpan time);
		void UpdateCollection(string role, TimeSpan time);
		void RecreateCollection(string role, TimeSpan time);
		void RemoveCollection(string role, TimeSpan time);
		void SecondLevelCachePut(string regionName);
		void SecondLevelCacheHit(string regionName);
		void SecondLevelCacheMiss(string regionName);
		void QueryExecuted(string hql, int rows, TimeSpan time);
		void QueryCacheHit(string hql, string regionName);
		void QueryCacheMiss(string hql, string regionName);
		void QueryCachePut(string hql, string regionName);
		void EndTransaction(bool success);
		void CloseStatement();
		void PrepareStatement();
		void OptimisticFailure(string entityName);
	}
}