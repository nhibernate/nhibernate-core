using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Persister;

namespace NHibernate.Loader.Criteria
{
	// TODO 6.0: Move members to ICriteriaInfoProvider and remove this.
	internal interface IExtendedCriteriaInfoProvider
	{
		IPersister Persister { get; }
	}
}
