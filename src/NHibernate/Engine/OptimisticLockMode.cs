using System;

namespace NHibernate.Engine
{
	public enum OptimisticLockMode
	{
		None = -1,
		Version = 0,
		Dirty = 1,
		All = 2
	}
}