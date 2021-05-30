using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Persister
{
	// TODO 6.0: Make this public and make IEntityPersister and ICollectionPersister derive it.
	internal interface IPersister
	{
		/// <summary>
		/// The unique name of the persister.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Whether the persister supports query cache.
		/// </summary>
		bool SupportsQueryCache { get; }
	}
}
