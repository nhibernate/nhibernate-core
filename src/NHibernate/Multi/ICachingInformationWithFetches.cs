using NHibernate.Type;

namespace NHibernate.Multi
{
	// 6.0 TODO: merge into 'ICachingInformation'.
	internal interface ICachingInformationWithFetches
	{
		/// <summary>
		/// The query cache types.
		/// </summary>
		IType[] CacheTypes { get; }
	}
}
