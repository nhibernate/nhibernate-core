using System;
using System.Linq;
using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.SqlCommand;

namespace NHibernate.Criterion
{
	[Serializable]
	public class ContainsExpression : AbstractCriterion
	{
		private readonly object value;
		private readonly IProjection projection;
		private readonly TypedValue typedValue;

		public ContainsExpression(IProjection projection, object value)
		{
			this.projection = projection;
			this.value = value;
			typedValue = new TypedValue(NHibernateUtil.String, this.value, false);
		}

		public ContainsExpression(string propertyName, object value)
			: this(Projections.Property(propertyName), value)
		{
		}

		public override SqlString ToSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			var columns = CriterionUtil.GetColumnNamesAsSqlStringParts(projection, criteriaQuery, criteria);
			var value = criteriaQuery.NewQueryParameter(typedValue).Single();
			var arguments = new[] { columns[0], value };
			var dialect = criteriaQuery.Factory.Dialect;
			var functionName = dialect.FullTextSearchFunction;
			
			var dialectFunction = criteriaQuery.Factory.SQLFunctionRegistry.FindSQLFunction(functionName);
			if (dialectFunction == null)
			{
				throw new HibernateException(string.Format("The current dialect '{0}' doesn't support the function: '{1}'",
					criteriaQuery.Factory.Dialect, functionName));
			}
			return dialectFunction.Render(arguments, criteriaQuery.Factory);
		}

		public override TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return new TypedValue[] { typedValue };
		}

		public override IProjection[] GetProjections()
		{
			if (projection != null)
			{
				return new IProjection[] { projection };
			}
			return null;
		}

		/// <summary></summary>
		public override string ToString()
		{
			return projection + " contains/freetext " + value;
		}
	}
}
