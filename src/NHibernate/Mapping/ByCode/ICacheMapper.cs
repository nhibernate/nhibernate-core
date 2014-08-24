namespace NHibernate.Mapping.ByCode
{
	public interface ICacheMapper
	{
		void Usage(CacheUsage cacheUsage);
		void Region(string regionName);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="cacheInclude"></param>
		/// <remarks>Not supported in NH3.</remarks>
		void Include(CacheInclude cacheInclude);
	}
}