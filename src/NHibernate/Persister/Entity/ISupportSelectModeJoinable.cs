using System;
using System.Collections.Generic;

namespace NHibernate.Persister.Entity
{
	// 6.0 TODO: merge into 'IJoinable'.
	public interface ISupportSelectModeJoinable
	{
		/// <summary>
		/// Given a query alias and an identifying suffix, render the identifier select fragment for joinable entity.
		/// </summary>
		string IdentifierSelectFragment(string name, string suffix);

		//Since 5.3
		/// <summary>
		/// All columns to select, when loading.
		/// </summary>
		[Obsolete("Please use overload taking EntityLoadInfo")]
		string SelectFragment(
			IJoinable rhs, string rhsAlias, string lhsAlias, string entitySuffix,
			string currentCollectionSuffix, bool includeCollectionColumns, bool includeLazyProperties);
	}

	public sealed class EntityLoadInfo
	{
		public bool IncludeLazyProps { get; set; }
		public string EntitySuffix { get; }
		public ISet<string> LazyProperties { get; set; }

		public EntityLoadInfo(string entitySuffix)
		{
			EntitySuffix = entitySuffix;
		}
	}

	// 6.0 TODO: merge into 'IJoinable'.
	internal interface ISupportLazyPropsJoinable
	{
		string SelectFragment(string lhsAlias, string collectionSuffix, bool includeCollectionColumns, EntityLoadInfo entityInfo);
	}
}
