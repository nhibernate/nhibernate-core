using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using NHibernate.Dialect;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Util;

namespace NHibernate.Driver
{
	/// <summary>
	/// A NHibernate Driver for using the Firebird data provider located in
	/// <c>FirebirdSql.Data.FirebirdClient</c> assembly.
	/// </summary>
	public class FirebirdClientDriver : ReflectionBasedDriver
	{
		private const string SELECT_CLAUSE_EXP = @"(?<=\bselect|\bwhere).*";
		private const string CAST_PARAMS_EXP = @"(?<![=<>]\s?|first\s?|skip\s?|between\s|between\s@\bp\w+\b\sand\s)@\bp\w+\b(?!\s?[=<>])";
		private readonly Regex _statementRegEx = new Regex(SELECT_CLAUSE_EXP, RegexOptions.IgnoreCase);
		private readonly Regex _castCandidateRegEx = new Regex(CAST_PARAMS_EXP, RegexOptions.IgnoreCase);
		private readonly FirebirdDialect _fbDialect = new FirebirdDialect();

		/// <summary>
		/// Initializes a new instance of the <see cref="FirebirdClientDriver"/> class.
		/// </summary>
		/// <exception cref="HibernateException">
		/// Thrown when the <c>FirebirdSql.Data.Firebird</c> assembly can not be loaded.
		/// </exception>
		public FirebirdClientDriver()
			: base(
				"FirebirdSql.Data.FirebirdClient",
				"FirebirdSql.Data.FirebirdClient",
				"FirebirdSql.Data.FirebirdClient.FbConnection",
				"FirebirdSql.Data.FirebirdClient.FbCommand")
		{

		}

		public override bool UseNamedPrefixInSql
		{
			get { return true; }
		}

		public override bool UseNamedPrefixInParameter
		{
			get { return true; }
		}

		public override string NamedPrefix
		{
			get { return "@"; }
		}

		protected override void InitializeParameter(DbParameter dbParam, string name, SqlType sqlType)
		{
			var convertedSqlType = sqlType;
			if (convertedSqlType.DbType == DbType.Currency)
				convertedSqlType = new SqlType(DbType.Decimal);

			base.InitializeParameter(dbParam, name, convertedSqlType);
		}

		public override DbCommand GenerateCommand(CommandType type, SqlString sqlString, SqlType[] parameterTypes)
		{
			var command = base.GenerateCommand(type, sqlString, parameterTypes);

			var expWithParams = GetStatementsWithCastCandidates(command.CommandText);
			if (!string.IsNullOrWhiteSpace(expWithParams))
			{
				var candidates = GetCastCandidates(expWithParams);
				var castParams = from DbParameter p in command.Parameters
								 where candidates.Contains(p.ParameterName)
								 select p;
				foreach (var param in castParams)
				{
					TypeCastParam(param, command);
				}
			}

			return command;
		}

		private string GetStatementsWithCastCandidates(string commandText)
		{
			return _statementRegEx.Match(commandText).Value;
		}

		private HashSet<string> GetCastCandidates(string statement)
		{
			var candidates =
				_castCandidateRegEx
					.Matches(statement)
					.Cast<Match>()
					.Select(match => match.Value);
			return new HashSet<string>(candidates);
		}

		private void TypeCastParam(DbParameter param, DbCommand command)
		{
			var castType = GetFbTypeFromDbType(param.DbType);
			command.CommandText = command.CommandText.ReplaceWholeWord(param.ParameterName, string.Format("cast({0} as {1})", param.ParameterName, castType));
		}

		private string GetFbTypeFromDbType(DbType dbType)
		{
			return _fbDialect.GetCastTypeName(new SqlType(dbType));
		}

		private static volatile MethodInfo _clearPool;
		private static volatile MethodInfo _clearAllPools;

		/// <summary>
		/// Clears the connection pool.
		/// </summary>
		/// <param name="connectionString">The connection string of connections for which to clear the pool.
		/// <c>null</c> for clearing them all.</param>
		public void ClearPool(string connectionString)
		{
			// In case of concurrent threads, may initialize many times. We do not care.
			// Members are volatile for avoiding it gets used while its constructor is not yet ended.
			if (_clearPool == null || _clearAllPools == null)
			{
				using (var clearConnection = CreateConnection())
				{
					var connectionType = clearConnection.GetType();
					_clearPool = connectionType.GetMethod("ClearPool") ?? throw new InvalidOperationException("Unable to resolve ClearPool method.");
					_clearAllPools = connectionType.GetMethod("ClearAllPools") ?? throw new InvalidOperationException("Unable to resolve ClearAllPools method.");
				}
			}

			if (connectionString != null)
			{
				using (var clearConnection = CreateConnection())
				{
					clearConnection.ConnectionString = connectionString;
					_clearPool.Invoke(null, new object[] {clearConnection});
				}
				return;
			}

			_clearAllPools.Invoke(null, new object[0]);
		}
	}
}
