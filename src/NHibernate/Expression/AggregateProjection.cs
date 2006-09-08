using System;
using System.Text;

using NHibernate.Type;
using NHibernate.SqlCommand;

namespace NHibernate.Expression
{
	/// <summary>
	/// An Aggregation
	/// </summary>
	public class AggregateProjection : SimpleProjection
	{
		protected readonly string propertyName;
		protected readonly string aggregate;

		protected internal AggregateProjection(string aggregate, string propertyName)
		{
			this.aggregate = aggregate;
			this.propertyName = propertyName;
		}

		public override string ToString()
		{
			return aggregate + "(" + propertyName + ')';
		}

		public override IType[] GetTypes(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return new IType[] { criteriaQuery.GetType(criteria, propertyName) };
		}

        public override SqlString ToSqlString(ICriteria criteria, int loc, ICriteriaQuery criteriaQuery)
        {
            return new SqlString(new object[] 
            {
				aggregate,
				"(",
				criteriaQuery.GetColumn(criteria, propertyName),
				") as y",
				loc.ToString(),
				"_"
            });
        }
	}
}
