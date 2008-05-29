using System.Collections;
using System.Collections.Generic;
using System.Text;
using NHibernate.Engine.Query;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;

namespace NHibernate.Loader.Custom.Sql
{
	public class SQLQueryParser
	{
		public interface IParserContext
		{
			bool IsEntityAlias(string aliasName);
			ISqlLoadable GetEntityPersisterByAlias(string alias);
			string GetEntitySuffixByAlias(string alias);
			bool IsCollectionAlias(string aliasName);
			ISqlLoadableCollection GetCollectionPersisterByAlias(string alias);
			string GetCollectionSuffixByAlias(string alias);
			IDictionary<string, string[]> GetPropertyResultsMapByAlias(string alias);
		}

		private readonly string originalQueryString;
		private readonly IParserContext context;

		private readonly Dictionary<string, object> namedParameters = new Dictionary<string, object>();

		private long aliasesFound = 0;

		public SQLQueryParser(string sqlQuery, IParserContext context)
		{
			originalQueryString = sqlQuery;
			this.context = context;
		}

		public IDictionary<string, object> NamedParameters
		{
			get { return namedParameters; }
		}

		public bool QueryHasAliases
		{
			get { return aliasesFound > 0; }
		}

		public SqlString Process()
		{
			return SubstituteParams(SubstituteBrackets());
		}

		// TODO: should "record" how many properties we have reffered to - and if we 
		//       don't get'em'all we throw an exception! Way better than trial and error ;)
		private string SubstituteBrackets()
		{
			StringBuilder result = new StringBuilder(originalQueryString.Length + 20);
			int right;

			// replace {....} with corresponding column aliases
			for (int curr = 0; curr < originalQueryString.Length; curr = right + 1)
			{
				int left;
				if ((left = originalQueryString.IndexOf('{', curr)) < 0)
				{
					// No additional open braces found in the string, Append the
					// rest of the string in its entirty and quit this loop
					result.Append(originalQueryString.Substring(curr));
					break;
				}

				// apend everything up until the next encountered open brace
				result.Append(originalQueryString.Substring(curr, left - curr));

				if ((right = originalQueryString.IndexOf('}', left + 1)) < 0)
				{
					throw new QueryException("Unmatched braces for alias path", originalQueryString);
				}

				string aliasPath = originalQueryString.Substring(left + 1, right - (left + 1));
				int firstDot = aliasPath.IndexOf('.');
				if (firstDot == -1)
				{
					if (context.IsEntityAlias(aliasPath))
					{
						// it is a simple table alias {foo}
						result.Append(aliasPath);
						aliasesFound++;
					}
					else
					{
						// passing through anything we do not know to support jdbc escape sequences HB-898
						result.Append('{').Append(aliasPath).Append('}');
					}
				}
				else
				{
					string aliasName = aliasPath.Substring(0, firstDot);
					bool isCollection = context.IsCollectionAlias(aliasName);
					bool isEntity = context.IsEntityAlias(aliasName);

					if (isCollection)
					{
						// The current alias is referencing the collection to be eagerly fetched
						string propertyName = aliasPath.Substring(firstDot + 1);
						result.Append(ResolveCollectionProperties(aliasName, propertyName));
						aliasesFound++;
					}
					else if (isEntity)
					{
						// it is a property reference {foo.bar}
						string propertyName = aliasPath.Substring(firstDot + 1);
						result.Append(ResolveProperties(aliasName, propertyName));
						aliasesFound++;
					}
					else
					{
						// passing through anything we do not know to support jdbc escape sequences HB-898
						result.Append('{').Append(aliasPath).Append('}');
					}
				}
			}

			// Possibly handle :something parameters for the query ?

			return result.ToString();
		}

		private string ResolveCollectionProperties(string aliasName, string propertyName)
		{
			IDictionary<string, string[]> fieldResults = context.GetPropertyResultsMapByAlias(aliasName);
			ISqlLoadableCollection collectionPersister = context.GetCollectionPersisterByAlias(aliasName);
			string collectionSuffix = context.GetCollectionSuffixByAlias(aliasName);

			if ("*".Equals(propertyName))
			{
				if (fieldResults.Count != 0)
				{
					throw new QueryException("Using return-propertys together with * syntax is not supported.");
				}

				string selectFragment = collectionPersister.SelectFragment(aliasName, collectionSuffix);
				aliasesFound++;
				return selectFragment + ", " + ResolveProperties(aliasName, propertyName);
			}
			else if ("element.*".Equals(propertyName))
			{
				return ResolveProperties(aliasName, "*");
			}
			else
			{
				string[] columnAliases;

				// Let return-propertys override whatever the persister has for aliases.
				if (!fieldResults.TryGetValue(propertyName,out columnAliases))
				{
					columnAliases = collectionPersister.GetCollectionPropertyColumnAliases(propertyName, collectionSuffix);
				}

				if (columnAliases == null || columnAliases.Length == 0)
				{
					throw new QueryException("No column name found for property [" + propertyName + "] for alias [" + aliasName + "]",
					                         originalQueryString);
				}
				if (columnAliases.Length != 1)
				{
					// TODO: better error message since we actually support composites if names are explicitly listed.
					throw new QueryException(
						"SQL queries only support properties mapped to a single column - property [" + propertyName + "] is mapped to "
						+ columnAliases.Length + " columns.", originalQueryString);
				}
				aliasesFound++;
				return columnAliases[0];
			}
		}

		private string ResolveProperties(string aliasName, string propertyName)
		{
			IDictionary<string, string[]> fieldResults = context.GetPropertyResultsMapByAlias(aliasName);
			ISqlLoadable persister = context.GetEntityPersisterByAlias(aliasName);
			string suffix = context.GetEntitySuffixByAlias(aliasName);

			if ("*".Equals(propertyName))
			{
				if (fieldResults.Count != 0)
				{
					throw new QueryException("Using return-propertys together with * syntax is not supported.");
				}
				aliasesFound++;
				return persister.SelectFragment(aliasName, suffix);
			}
			else
			{
				string[] columnAliases;

				// Let return-propertys override whatever the persister has for aliases.
				if (!fieldResults.TryGetValue(propertyName, out columnAliases))
				{
					columnAliases = persister.GetSubclassPropertyColumnAliases(propertyName, suffix);
				}

				if (columnAliases == null || columnAliases.Length == 0)
				{
					throw new QueryException("No column name found for property [" + propertyName + "] for alias [" + aliasName + "]",
					                         originalQueryString);
				}
				if (columnAliases.Length != 1)
				{
					// TODO: better error message since we actually support composites if names are explicitly listed.
					throw new QueryException(
						"SQL queries only support properties mapped to a single column - property [" + propertyName + "] is mapped to "
						+ columnAliases.Length + " columns.", originalQueryString);
				}
				aliasesFound++;
				return columnAliases[0];
			}
		}

		/// <summary> 
		/// Substitues ADO parameter placeholders (?) for all encountered
		/// parameter specifications.  It also tracks the positions of these
		/// parameter specifications within the query string.  This accounts for
		/// ordinal-params, named-params, and ejb3-positional-params.
		///  </summary>
		/// <param name="sqlString">The query string. </param>
		/// <returns> The SQL query with parameter substitution complete. </returns>
		private SqlString SubstituteParams(string sqlString)
		{
			ParameterSubstitutionRecognizer recognizer = new ParameterSubstitutionRecognizer();
			ParameterParser.Parse(sqlString, recognizer);

			namedParameters.Clear();
			foreach (KeyValuePair<string, object> de in recognizer.namedParameterBindPoints)
			{
				namedParameters.Add(de.Key, de.Value);
			}

			return recognizer.result.ToSqlString();
		}

		public class ParameterSubstitutionRecognizer : ParameterParser.IRecognizer
		{
			internal SqlStringBuilder result = new SqlStringBuilder();
			internal Dictionary<string, object> namedParameterBindPoints = new Dictionary<string, object>();
			internal int parameterCount = 0;

			public void OutParameter(int position)
			{
				result.Add(Parameter.Placeholder);
			}

			public void OrdinalParameter(int position)
			{
				result.Add(Parameter.Placeholder);
			}

			public void NamedParameter(string name, int position)
			{
				AddNamedParameter(name);
				result.Add(Parameter.Placeholder);
			}

			public void JpaPositionalParameter(string name, int position)
			{
				NamedParameter(name, position);
			}

			public void Other(char character)
			{
				result.Add(character.ToString());
			}

			private void AddNamedParameter(string name)
			{
				int loc = parameterCount++;
				object o;
				if (!namedParameterBindPoints.TryGetValue(name, out o))
				{
					namedParameterBindPoints[name] = loc;
				}
				else if (o is int)
				{
					List<int> list = new List<int>(4);
					list.Add((int) o);
					list.Add(loc);
					namedParameterBindPoints[name] = list;
				}
				else
				{
					((IList) o).Add(loc);
				}
			}
		}
	}
}