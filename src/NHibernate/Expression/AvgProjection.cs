using System;

using NHibernate.Type;

namespace NHibernate.Expression
{
	public class AvgProjection : AggregateProjection
	{

		public AvgProjection(String propertyName)
			: base("avg", propertyName)
		{
		}

		public override IType[] GetTypes(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return new IType[] { NHibernateUtil.Double };
		}
	}
}
using System;

using NHibernate.Type;

namespace NHibernate.Expression
{
	public class AvgProjection : AggregateProjection
	{

		public AvgProjection(String propertyName)
			: base("avg", propertyName)
		{
		}

		public override IType[] GetTypes(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return new IType[] { NHibernateUtil.Double };
		}
	}
}
