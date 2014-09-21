using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Mapping.ByCode
{
	public enum OptimisticLockMode
	{
		None,

		Version,

		Dirty,

		All,
	}
}
