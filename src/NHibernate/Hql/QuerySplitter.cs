using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NHibernate.Engine;
using NHibernate.Hql.Util;
using NHibernate.Util;

namespace NHibernate.Hql
{
	public class QuerySplitter
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(QuerySplitter));

		private static readonly HashSet<string> beforeClassTokens = new HashSet<string>();
		private static readonly HashSet<string> notAfterClassTokens = new HashSet<string>();

		static QuerySplitter()
		{
			beforeClassTokens.Add("from");
			beforeClassTokens.Add("delete");
			beforeClassTokens.Add("update");
			//beforeClassTokens.Add("new"); DEFINITELY DON'T HAVE THIS!! (form H3.2)
			beforeClassTokens.Add(",");
			notAfterClassTokens.Add("in");
			//notAfterClassTokens.Add(",");
			notAfterClassTokens.Add("from");
			notAfterClassTokens.Add(")");
		}

		/// <summary>
		/// Handle Hibernate "implicit" polymorphism, by translating the query string into 
		/// several "concrete" queries against mapped classes.
		/// </summary>
		/// <param name="query"></param>
		/// <param name="factory"></param>
		/// <returns></returns>
		/// <exception cref="NHibernate.MappingException"/>
		public static string[] ConcreteQueries(string query, ISessionFactoryImplementor factory)
		{
			//scan the query string for class names appearing in the from clause and replace
			//with all persistent implementors of the class/interface, returning multiple
			//query strings (make sure we don't pick up a class in the select clause!)

			//TODO: this is one of the ugliest and most fragile pieces of code in Hibernate....

			SessionFactoryHelper helper = new SessionFactoryHelper(factory);

			string[] tokens = StringHelper.Split(StringHelper.WhiteSpace + "(),", query, true);
			if (tokens.Length == 0)
			{
				return new String[] {query}; // just especially for the trivial collection filter
			}
			var placeholders = new List<object>();
			var replacements = new List<object>();
			StringBuilder templateQuery = new StringBuilder(40);
			int count = 0;
			string last = null;
			int nextIndex = 0;
			string next = null;

			templateQuery.Append(tokens[0]);
			bool isSelectClause = StringHelper.EqualsCaseInsensitive("select", tokens[0]);

			for (int i = 1; i < tokens.Length; i++)
			{
				//update last non-whitespace token, if necessary
				if (!ParserHelper.IsWhitespace(tokens[i - 1]))
				{
					last = tokens[i - 1].ToLowerInvariant();
				}

				// select-range is terminated by declaration of "from"
				isSelectClause = !StringHelper.EqualsCaseInsensitive("from", tokens[i]);

				string token = tokens[i];
				if (!ParserHelper.IsWhitespace(token) || last == null)
				{
					//scan for next non-whitespace token
					if (nextIndex <= i)
					{
						for (nextIndex = i + 1; nextIndex < tokens.Length; nextIndex++)
						{
							next = tokens[nextIndex].ToLowerInvariant();
							if (!ParserHelper.IsWhitespace(next))
							{
								break;
							}
						}
					}

					// TODO H3.2 Different behavior
					// NHb: This block is not an exactly port from H3.2 but a port from previous implementation of QueryTranslator
					if (((last != null && beforeClassTokens.Contains(last)) &&
						 (next == null || !notAfterClassTokens.Contains(next))) ||
						ParserHelper.EntityClass.Equals(last))
					{
						System.Type clazz = helper.GetImportedClass(token);
						if (clazz != null)
						{
							string[] implementors = factory.GetImplementors(clazz.FullName);
							string placeholder = "$clazz" + count++ + "$";

							if (implementors != null)
							{
								placeholders.Add(placeholder);
								replacements.Add(implementors);
							}
							token = placeholder; //Note this!!
						}
					}
				}
				templateQuery.Append(token);
			}
			string[] results =
				StringHelper.Multiply(templateQuery.ToString(), placeholders.GetEnumerator(), replacements.GetEnumerator());
			if (results.Length == 0)
			{
				log.Warn("no persistent classes found for query class: " + query);
			}
			return results;
		}

		private static bool IsPossiblyClassName(string last, string next)
		{
			return ParserHelper.EntityClass.Equals(last) ||
				   (beforeClassTokens.Contains(last) && !notAfterClassTokens.Contains(next));
		}
	}
}