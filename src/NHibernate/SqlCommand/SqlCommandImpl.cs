using System.Collections.Generic;
using System.Data;
using System.Linq;
using NHibernate.Engine;
using NHibernate.Param;
using NHibernate.SqlTypes;

namespace NHibernate.SqlCommand
{
	public interface ISqlCommand
	{
		SqlType[] ParameterTypes { get; }
		SqlString Query { get; }
		QueryParameters QueryParameters { get; }

		/// <summary>
		/// Bind the appropriate value into the given command.
		/// </summary>
		/// <param name="command">The command into which the value should be bound.</param>
		/// <param name="commandQueryParametersList">The parameter-list of the whole query of the command.</param>
		/// <param name="singleSqlParametersOffset">The offset from where start the list of <see cref="IDataParameter"/>, in the given <paramref name="command"/>, for the this <see cref="SqlCommandImpl"/>. </param>
		/// <param name="session">The session against which the current execution is occuring.</param>
		void Bind(IDbCommand command, IList<Parameter> commandQueryParametersList, int singleSqlParametersOffset, ISessionImplementor session);
	}

	public class SqlCommandImpl : ISqlCommand
	{
		private readonly SqlString query;
		private readonly ICollection<IParameterSpecification> specifications;
		private readonly QueryParameters queryParameters;
		private readonly ISessionFactoryImplementor factory;
		private SqlType[] parameterTypes;
		List<Parameter> sqlQueryParametersList;

		public SqlCommandImpl(SqlString query, ICollection<IParameterSpecification> specifications, QueryParameters queryParameters, ISessionFactoryImplementor factory)
		{
			this.query = query;
			this.specifications = specifications;
			this.queryParameters = queryParameters;
			this.factory = factory;
		}

		private List<Parameter> SqlQueryParametersList
		{
			get { return sqlQueryParametersList ?? (sqlQueryParametersList = query.GetParameters().ToList()); }
		}

		public SqlType[] ParameterTypes
		{
			get { return parameterTypes ?? (parameterTypes = specifications.GetQueryParameterTypes(SqlQueryParametersList, factory)); }
		}

		public SqlString Query
		{
			get { return query; }
		}

		public IEnumerable<IParameterSpecification> Specifications
		{
			get { return specifications; }
		}

		public QueryParameters QueryParameters
		{
			get { return queryParameters; }
		}

		/// <summary>
		/// Bind the appropriate value into the given command.
		/// </summary>
		/// <param name="command">The command into which the value should be bound.</param>
		/// <param name="commandQueryParametersList">The parameter-list of the whole query of the command.</param>
		/// <param name="singleSqlParametersOffset">The offset from where start the list of <see cref="IDataParameter"/>, in the given <paramref name="command"/>, for the this <see cref="SqlCommandImpl"/>. </param>
		/// <param name="session">The session against which the current execution is occuring.</param>
		public void Bind(IDbCommand command, IList<Parameter> commandQueryParametersList, int singleSqlParametersOffset, ISessionImplementor session)
		{
			foreach (IParameterSpecification parameterSpecification in Specifications)
			{
				parameterSpecification.Bind(command, commandQueryParametersList, singleSqlParametersOffset, SqlQueryParametersList, QueryParameters, session);
			}
		}
	}
}