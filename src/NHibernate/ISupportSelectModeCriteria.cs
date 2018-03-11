using NHibernate.Loader;

namespace NHibernate
{
	// 6.0 TODO: merge into 'ICriteria'.
	public interface ISupportSelectModeCriteria
	{
		/// <summary>
		/// Applies <paramref name="selectMode"/> for criteria with given <paramref name="alias"/> and given <paramref name="associationPath"/>
		/// </summary>
		/// <param name="selectMode"></param>
		/// <param name="associationPath">Criteria association path. If empty root entity for given criteria is used.</param>
		/// <param name="alias">Criteria alias. If empty current <see cref="ICriteria"/> criteria is used.</param>
		ICriteria SetSelectMode(SelectMode selectMode, string associationPath, string alias);
	}
}
