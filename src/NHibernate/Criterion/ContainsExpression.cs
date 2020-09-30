using System.Collections.Generic;
using System.Linq;
using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Util;

namespace NHibernate.Criterion
{
	public class ContainsExpression : AbstractCriterion
	{
		private readonly object value;
		private readonly IProjection projection;
		private readonly TypedValue typedValue;
		private readonly bool freetext;

		public ContainsExpression(IProjection projection, object value, bool freetext)
		{
			this.projection = projection;
			this.value = value;
			typedValue = new TypedValue(NHibernateUtil.String, this.value, false);
			this.freetext = freetext;
		}

		public ContainsExpression(string propertyName, object value, bool freetext)
			: this(Projections.Property(propertyName), value, freetext)
		{
		}

		public override SqlString ToSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			var columns = CriterionUtil.GetColumnNamesAsSqlStringParts(projection, criteriaQuery, criteria);
			var value = criteriaQuery.NewQueryParameter(typedValue).Single();
			var arguments = new[] { columns[0], value };
			var functionName = GetFunctionName(criteriaQuery);

			var dialectFunction = criteriaQuery.Factory.SQLFunctionRegistry.FindSQLFunction(functionName);
			if (dialectFunction == null)
			{
				throw new HibernateException(string.Format("The current dialect '{0}' doesn't support the function: '{1}'",
					criteriaQuery.Factory.Dialect, functionName));
			}
			return dialectFunction.Render(arguments, criteriaQuery.Factory);
		}

		private string GetFunctionName(ICriteriaQuery criteriaQuery)
		{
			var dialect = criteriaQuery.Factory.Dialect;

			if (dialect is MsSql2000Dialect)
				return freetext ? "freetext" : "contains";

			if (dialect is SQLiteDialect)
				return "match";

			return "contains";
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
