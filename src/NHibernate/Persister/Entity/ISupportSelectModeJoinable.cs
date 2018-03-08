namespace NHibernate.Persister.Entity
{
	// 6.0 TODO: merge into 'IJoinable'.
	public interface ISupportSelectModeJoinable
	{
		/// <summary>
		/// Given a query alias and an identifying suffix, render the identifier select fragment for joinable entity.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="suffix"></param>
		/// <returns></returns>
		string IdentifierSelectFragment(string name, string suffix);

		/// <summary>
		/// All columns to select, when loading.
		/// </summary>
		string SelectFragment(IJoinable rhs, string rhsAlias, string lhsAlias, string entitySuffix,
							string currentCollectionSuffix, bool includeCollectionColumns, bool includeLazyProperties);

	}
}
