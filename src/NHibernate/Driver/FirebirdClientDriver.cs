using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using NHibernate.Dialect;
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
		private const string SELECT_CLAUSE_EXP = "(?<=select|where).*";
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

		public override void AdjustCommand(IDbCommand command)
		{
			var expWithParams = GetStatementsWithCastCandidates(command.CommandText);
			if (string.IsNullOrWhiteSpace(expWithParams))
				return;

			var candidates = GetCastCandidates(expWithParams);
			var castParams = from IDbDataParameter p in command.Parameters
							 where candidates.Contains(p.ParameterName)
							 select p;
			foreach (IDbDataParameter param in castParams)
			{
				TypeCastParam(param, command);
			}
		}

		private string GetStatementsWithCastCandidates(string commandText)
		{
			var match = _statementRegEx.Match(commandText);
			return match.Value;
		}

		private IEnumerable<string> GetCastCandidates(string statement)
		{
			var matches = _castCandidateRegEx.Matches(statement);
			foreach (Match match in matches)
			{
				yield return match.Value;
			}
		}

		private void TypeCastParam(IDbDataParameter param, IDbCommand command)
		{
			var castType = GetFbTypeFromDbType(param.DbType);
			command.CommandText = command.CommandText.ReplaceWholeWord(param.ParameterName, string.Format("cast({0} as {1})", param.ParameterName, castType));
		}

		private string GetFbTypeFromDbType(DbType dbType)
		{
			return _fbDialect.GetCastTypeName(new SqlType(dbType));
		}
	}
}