using System;
using NHibernate.Type;

namespace NHibernate.Criterion
{
	[Serializable]
	public class RowCountInt64Projection : RowCountProjection
	{
		public override IType[] GetTypes(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return new IType[] { NHibernateUtil.Int64 };
		}
	}
}