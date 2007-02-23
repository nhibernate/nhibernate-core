using System;
using NHibernate.Type;

namespace NHibernate.Expression
{
	[Serializable]
	public class AvgProjection : AggregateProjection
	{
		public AvgProjection(String propertyName)
			: base("avg", propertyName)
		{
		}

		public override IType[] GetTypes(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return new IType[] {NHibernateUtil.Double};
		}
	}
}