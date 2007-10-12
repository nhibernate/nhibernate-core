using System;
using Iesi.Collections.Generic;
using NHibernate.Type;

namespace NHibernate.Engine.Query
{
	/// <summary> Defines metadata regarding a translated HQL or native-SQL query. </summary>
	[Serializable]
	public class QueryMetadata
	{
		private readonly string sourceQuery;
		private readonly ParameterMetadata parameterMetadata;
		private readonly string[] returnAliases;
		private readonly IType[] returnTypes;
		private readonly ISet<string> querySpaces;

		public QueryMetadata(string sourceQuery, ParameterMetadata parameterMetadata, 
			string[] returnAliases, IType[] returnTypes, ISet<string> querySpaces)
		{
			this.sourceQuery = sourceQuery;
			this.parameterMetadata = parameterMetadata;
			this.returnAliases = returnAliases;
			this.returnTypes = returnTypes;
			this.querySpaces = querySpaces;
		}

		/// <summary> Get the source HQL or native-SQL query. </summary>
		public string SourceQuery
		{
			get { return sourceQuery; }
		}

		public ParameterMetadata ParameterMetadata
		{
			get { return parameterMetadata; }
		}

		/// <summary> Return source query select clause aliases (if any) </summary>
		public string[] ReturnAliases
		{
			get { return returnAliases; }
		}

		/// <summary> An array of types describing the returns of the source query. </summary>
		public IType[] ReturnTypes
		{
			get { return returnTypes; }
		}

		/// <summary> The set of query spaces affected by this source query. </summary>
		public ISet<string> QuerySpaces
		{
			get { return querySpaces; }
		}
	}
}
