using System.Collections;
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
			IDictionary GetPropertyResultsMapByAlias(string alias);
		}

		private readonly string sqlQuery;
		private IParserContext context;

		private readonly IDictionary namedParameters = new Hashtable();

		private long aliasesFound = 0;

		public SQLQueryParser(
			string sqlQuery,
			IParserContext context)
		{
			this.sqlQuery = sqlQuery;
			this.context = context;
		}

		public IDictionary NamedParameters
		{
			get { return namedParameters; }
		}

		public bool QueryHasAliases
		{
			get { return aliasesFound > 0; }
		}

		public SqlString Process()
		{
			return substituteParams(SubstituteBrackets());
		}

		// TODO: should "record" how many properties we have reffered to - and if we 
		//       don't get'em'all we throw an exception! Way better than trial and error ;)
		private string SubstituteBrackets()
		{
			StringBuilder result = new StringBuilder(sqlQuery.Length + 20);
			int left, right;

			// replace {....} with corresponding column aliases
			for (int curr = 0; curr < sqlQuery.Length; curr = right + 1)
			{
				if ((left = sqlQuery.IndexOf('{', curr)) < 0)
				{
					// No additional open braces found in the string, Append the
					// rest of the string in its entirty and quit this loop
					result.Append(sqlQuery.Substring(curr));
					break;
				}

				// apend everything up until the next encountered open brace
				result.Append(sqlQuery.Substring(curr, left - curr));

				if ((right = sqlQuery.IndexOf('}', left + 1)) < 0)
				{
					throw new QueryException("Unmatched braces for alias path", sqlQuery);
				}

				string aliasPath = sqlQuery.Substring(left + 1, right - (left + 1));
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
						result.Append(
							ResolveCollectionProperties(
								aliasName,
								propertyName,
								context.GetPropertyResultsMapByAlias(aliasName),
								context.GetEntityPersisterByAlias(aliasName),
								context.GetCollectionPersisterByAlias(aliasName),
								context.GetCollectionSuffixByAlias(aliasName),
								context.GetEntitySuffixByAlias(aliasName)
								)
							);
						aliasesFound++;
					}
					else if (isEntity)
					{
						// it is a property reference {foo.bar}
						string propertyName = aliasPath.Substring(firstDot + 1);
						result.Append(
							ResolveProperties(
								aliasName,
								propertyName,
								context.GetPropertyResultsMapByAlias(aliasName),
								context.GetEntityPersisterByAlias(aliasName),
								context.GetEntitySuffixByAlias(aliasName)
								)
							);
						aliasesFound++;
					}

					if (!isEntity && !isCollection)
					{
						// passing through anything we do not know to support jdbc escape sequences HB-898
						result.Append('{').Append(aliasPath).Append('}');
					}
				}
			}

			// Possibly handle :something parameters for the query ?

			return result.ToString();
		}

		private string ResolveCollectionProperties(
			string aliasName,
			string propertyName,
			IDictionary fieldResults,
			ISqlLoadable elementPersister,
			ISqlLoadableCollection currentPersister,
			string suffix,
			string persisterSuffix)
		{
			if ("*".Equals(propertyName))
			{
				if (fieldResults.Count > 0)
				{
					throw new QueryException("Using return-propertys together with * syntax is not supported.");
				}

				string selectFragment = currentPersister.SelectFragment(aliasName, suffix);
				aliasesFound++;
				return selectFragment
				       + ", "
				       + ResolveProperties(aliasName, propertyName, fieldResults, elementPersister, persisterSuffix);
			}
			else if ("element.*".Equals(propertyName))
			{
				return ResolveProperties(aliasName, "*", fieldResults, elementPersister, persisterSuffix);
			}
			else
			{
				string[] columnAliases;

				// Let return-propertys override whatever the persister has for aliases.
				columnAliases = (string[]) fieldResults[propertyName];
				if (columnAliases == null)
				{
					columnAliases = currentPersister.GetCollectionPropertyColumnAliases(propertyName, suffix);
				}

				if (columnAliases == null || columnAliases.Length == 0)
				{
					throw new QueryException("No column name found for property [" +
					                         propertyName +
					                         "] for alias [" + aliasName + "]",
					                         sqlQuery);
				}
				if (columnAliases.Length != 1)
				{
					// TODO: better error message since we actually support composites if names are explicitly listed.
					throw new QueryException("SQL queries only support properties mapped to a single column - property [" +
					                         propertyName +
					                         "] is mapped to " +
					                         columnAliases.Length +
					                         " columns.",
					                         sqlQuery);
				}
				aliasesFound++;
				return columnAliases[0];
			}
		}

		private string ResolveProperties(
			string aliasName,
			string propertyName,
			IDictionary fieldResults,
			ISqlLoadable currentPersister,
			string suffix)
		{
			/*int currentPersisterIndex = getPersisterIndex( aliasName );

			if ( !aliasName.Equals( aliases[currentPersisterIndex] ) ) {
				throw new QueryException( "Alias [" +
						aliasName +
						"] does not correspond to return alias " +
						aliases[currentPersisterIndex],
						sqlQuery );
			}*/

			if ("*".Equals(propertyName))
			{
				if (fieldResults.Count > 0)
				{
					throw new QueryException("Using return-propertys together with * syntax is not supported.");
				}
				aliasesFound++;
				return currentPersister.SelectFragment(aliasName, suffix);
			}
			else
			{
				string[] columnAliases;

				// Let return-propertys override whatever the persister has for aliases.
				columnAliases = (string[]) fieldResults[propertyName];
				if (columnAliases == null)
				{
					columnAliases = currentPersister.GetSubclassPropertyColumnAliases(propertyName, suffix);
				}

				if (columnAliases == null || columnAliases.Length == 0)
				{
					throw new QueryException("No column name found for property [" +
					                         propertyName +
					                         "] for alias [" + aliasName + "]",
					                         sqlQuery);
				}
				if (columnAliases.Length != 1)
				{
					// TODO: better error message since we actually support composites if names are explicitly listed.
					throw new QueryException("SQL queries only support properties mapped to a single column - property [" +
					                         propertyName +
					                         "] is mapped to " +
					                         columnAliases.Length +
					                         " columns.",
					                         sqlQuery);
				}
				aliasesFound++;
				return columnAliases[0];
			}
		}

		/**
		 * Substitues JDBC parameter placeholders (?) for all encountered
		 * parameter specifications.  It also tracks the positions of these
		 * parameter specifications within the query string.  This accounts for
		 * ordinal-params, named-params, and ejb3-positional-params.
		 *
		 * @param sqlString The query string.
		 * @return The SQL query with parameter substitution complete.
		 */

		private SqlString substituteParams(string sqlString)
		{
			ParameterSubstitutionRecognizer recognizer = new ParameterSubstitutionRecognizer();
			ParameterParser.Parse(sqlString, recognizer);

			namedParameters.Clear();
			foreach (DictionaryEntry de in recognizer.namedParameterBindPoints)
			{
				namedParameters.Add(de.Key, de.Value);
			}

			return recognizer.result.ToSqlString();
		}

		public class ParameterSubstitutionRecognizer : ParameterParser.IRecognizer
		{
			internal SqlStringBuilder result = new SqlStringBuilder();
			internal IDictionary namedParameterBindPoints = new Hashtable();
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

			public void Ejb3PositionalParameter(string name, int position)
			{
				NamedParameter(name, position);
			}

			public void Other(char character)
			{
				result.Add(new string(character, 1));
			}

			private void AddNamedParameter(string name)
			{
				int loc = parameterCount++;
				object o = namedParameterBindPoints[name];
				if (o == null)
				{
					namedParameterBindPoints[name] = loc;
				}
				else if (o is int)
				{
					ArrayList list = new ArrayList(4);
					list.Add(o);
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