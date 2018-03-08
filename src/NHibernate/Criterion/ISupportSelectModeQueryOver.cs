using System;
using System.Linq.Expressions;
using NHibernate.Loader;

namespace NHibernate.Criterion
{
	// 6.0 TODO: merge into IQueryOver<TRoot,TSubType>.
	public interface ISupportSelectModeQueryOver<TRoot, TSubType>
	{
		IQueryOver<TRoot, TSubType> SetSelectMode(SelectMode mode, Expression<Func<TSubType, object>> path);
	}
}
