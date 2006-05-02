using System;
using System.Collections;

using Iesi.Collections;

using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Loader.Custom
{
	public interface ICustomQuery
	{
		SqlString SQL { get; }
		ISet QuerySpaces { get; }

		/// <summary>
		/// Optional, may return <c>null</c>
		/// </summary>
		IDictionary NamedParameterBindPoints { get; }

		System.Type[] EntityNames { get; }
		IEntityAliases[] EntityAliases { get; }
		ICollectionAliases[] CollectionAliases { get; }
		LockMode[] LockModes { get; }

		/// <summary>
		/// Optional, may return <c>null</c>
		/// </summary>
		int[] EntityOwners { get; }

		/// <summary>
		/// Optional, may return <c>null</c>
		/// </summary>
		int[] CollectionOwner { get; }

		/// <summary>
		/// Optional, may return <c>null</c>
		/// </summary>
		string[] CollectionRoles { get; }

		/// <summary>
		/// Optional, may return <c>null</c>
		/// </summary>
		IType[] ScalarTypes { get; }

		/// <summary>
		/// Optional, may return <c>null</c>
		/// </summary>
		string[] ScalarColumnAliases { get; }

		/// <summary>
		/// Optional, may return <c>null</c>
		/// </summary>
		string[] ReturnAliases { get; }
	}
}
