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
		/// re-set the index of each parameter in the final <see cref="IDbCommand">command</see>.
		/// </summary>
		/// <param name="singleSqlParametersOffset">The offset from where start the list of <see cref="IDataParameter"/>, in the given command, for the this <see cref="SqlCommandImpl"/>. </param>
		/// <remarks>
		/// Suppose the final <see cref="IDbCommand">command</see> is composed by two queries. The <paramref name="singleSqlParametersOffset"/> for the first query is zero.
		/// If the first query command has 12 parameters (size of its SqlType array) the offset to bind all <see cref="IParameterSpecification"/>s, of the second query in the
		/// command, is 12 (for the first query we are using from 0 to 11).
		/// <para>
		/// This method should be called before call <see cref="IBatcher.PrepareCommand"/>.
		/// </para>
		/// </remarks>
		void ResetParametersIndexesForTheCommand(int singleSqlParametersOffset); // Note: should be included ParameterTypes getter

		/// <summary>
		/// Bind the appropriate value into the given command.
		/// </summary>
		/// <param name="command">The command into which the value should be bound.</param>
		/// <param name="commandQueryParametersList">The parameter-list of the whole query of the command.</param>
		/// <param name="singleSqlParametersOffset">The offset from where start the list of <see cref="IDataParameter"/>, in the given <paramref name="command"/>, for the this <see cref="SqlCommandImpl"/>. </param>
		/// <param name="session">The session against which the current execution is occuring.</param>
		/// <remarks>
		/// Suppose the <paramref name="command"/> is composed by two queries. The <paramref name="singleSqlParametersOffset"/> for the first query is zero.
		/// If the first query in <paramref name="command"/> has 12 parameters (size of its SqlType array) the offset to bind all <see cref="IParameterSpecification"/>s, of the second query in the
		/// <paramref name="command"/>, is 12 (for the first query we are using from 0 to 11).
		/// </remarks>
		void Bind(IDbCommand command, IList<Parameter> commandQueryParametersList, int singleSqlParametersOffset, ISessionImplementor session);

		/// <summary>
		/// Bind the appropriate value into the given command.
		/// </summary>
		/// <param name="command">The command into which the value should be bound.</param>
		/// <param name="session">The session against which the current execution is occuring.</param>
		/// <remarks>
		/// Use this method when the <paramref name="command"/> contains just 'this' instance of <see cref="ISqlCommand"/>.
		/// Use the overload <see cref="Bind(IDbCommand, IList{Parameter}, int, ISessionImplementor)"/> when the <paramref name="command"/> contains more instances of <see cref="ISqlCommand"/>.
		/// </remarks>
		void Bind(IDbCommand command, ISessionImplementor session);
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

		public List<Parameter> SqlQueryParametersList
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

		public void ResetParametersIndexesForTheCommand(int singleSqlParametersOffset)
		{
			// a better place could be the Bind of each IParameterSpecification but we have to do it before bind values
			// in this way the same parameter of a dynamic-filter will be set with two different parameter-names in the same command (when it is a command-set). 
			if (singleSqlParametersOffset < 0)
			{
				throw new AssertionFailure("singleSqlParametersOffset < 0 - this indicate a bug in NHibernate ");
			}
			// due to IType.NullSafeSet(System.Data.IDbCommand , object, int, ISessionImplementor) the SqlType[] is supposed to be in a certain sequence.
			// this mean that found the first location of a parameter for the IType span, the others are in secuence
			foreach (IParameterSpecification specification in Specifications)
			{
				string firstParameterId = specification.GetIdsForBackTrack(factory).First();
				int[] effectiveParameterLocations = SqlQueryParametersList.GetEffectiveParameterLocations(firstParameterId).ToArray();
				if (effectiveParameterLocations.Length > 0) // Parameters previously present might have been removed from the SQL at a later point.
				{
					int firstParamNameIndex = effectiveParameterLocations.First() + singleSqlParametersOffset;
					foreach (int location in effectiveParameterLocations)
					{
						int parameterSpan = specification.ExpectedType.GetColumnSpan(factory);
						for (int j = 0; j < parameterSpan; j++)
						{
							sqlQueryParametersList[location + j].ParameterPosition = firstParamNameIndex + j;
						}
					}
				}
			}
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

		/// <summary>
		/// Bind the appropriate value into the given command.
		/// </summary>
		/// <param name="command">The command into which the value should be bound.</param>
		/// <param name="session">The session against which the current execution is occuring.</param>
		/// <remarks>
		/// Use this method when the <paramref name="command"/> contains just 'this' instance of <see cref="ISqlCommand"/>.
		/// Use the overload <see cref="Bind(IDbCommand, IList{Parameter}, int, ISessionImplementor)"/> when the <paramref name="command"/> contains more instances of <see cref="ISqlCommand"/>.
		/// </remarks>
		public void Bind(IDbCommand command, ISessionImplementor session)
		{
			foreach (IParameterSpecification parameterSpecification in Specifications)
			{
				parameterSpecification.Bind(command, SqlQueryParametersList, QueryParameters, session);
			}
		}
	}
}