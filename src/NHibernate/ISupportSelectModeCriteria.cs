using NHibernate.Loader;

namespace NHibernate
{
	// 6.0 TODO: merge into 'ICriteria'.
	public interface ISupportSelectModeCriteria
	{
		/// <summary>
		/// Applies <paramref name="selectMode"/> for the criteria with the given <paramref name="alias"/> and the
		/// given <paramref name="associationPath"/>.
		/// </summary>
		/// <param name="selectMode">The select mode to apply.</param>
		/// <param name="associationPath">The criteria association path. If empty, the root entity for the given
		/// criteria is used.</param>
		/// <param name="alias">The criteria alias. If empty, the current <see cref="ICriteria"/> criteria is used.</param>
		ICriteria SetSelectMode(SelectMode selectMode, string associationPath, string alias);
	}
}
