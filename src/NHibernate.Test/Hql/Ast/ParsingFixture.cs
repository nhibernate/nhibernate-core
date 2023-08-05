using System;
using System.Collections.Generic;
using System.Xml.Linq;
using NHibernate.Hql.Ast.ANTLR;
using NUnit.Framework;

namespace NHibernate.Test.Hql.Ast
{
	[TestFixture]
	public class ParsingFixture
	{
		/// <summary>
		/// Key test for HQL strings -> HQL AST's - takes the query and returns a string
		/// representation of the resultant tree
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		[Test]
		[TestCaseSource(typeof(QueryFactoryClass), nameof(QueryFactoryClass.TestCases))]
		public string HqlParse(string query)
		{
			var p = new HqlParseEngine(query, false, null);
			var result = p.Parse().ToStringTree();

			return " " + result;
		}

		/// <summary>
		/// Used to test individual queries "by hand", since td.net doesn't let me run a 
		/// single test out of a data set
		/// </summary>
		[Test, Explicit]
		public void ManualTest()
		{
			var p = new HqlParseEngine(@"select all s, s.Other from s in class Simple where s = :s", false, null);

			var result = p.Parse().ToStringTree();

			Console.WriteLine(result);
		}

		/// <summary>
		/// Helper "test" to display queries that are ignored
		/// </summary>
		[Test]
		public void ShowIgnoredQueries()
		{
			foreach (string query in QueryFactoryClass.GetIgnores)
			{
				Console.WriteLine(query);
			}
		}

		/// <summary>
		/// Helper "test" to display queries that don't parse in H3
		/// </summary>
		[Test]
		public void ShowExceptionQueries()
		{
			foreach (string query in QueryFactoryClass.GetExceptions)
			{
				Console.WriteLine(query);
			}
		}

		/// <summary>
		/// Class used by Nunit 2.5 to drive the data into the HqlParse test
		/// </summary>
		public class QueryFactoryClass
		{
			public static IEnumerable<TestCaseData> TestCases
			{
				get
				{
					// TODO - need to handle Ignore better (it won't show in results...)
					return EnumerateTests(
						td => !td.Ignore && !td.Result.StartsWith("Exception"),
						td => new TestCaseData(td.Query)
						      .Returns(td.Result)
						      .SetCategory(td.Category)
						      .SetName(td.Name)
						      .SetDescription(td.Description));
				}
			}

			public static IEnumerable<string> GetIgnores
			{
				get
				{
					return EnumerateTests(
						td => td.Ignore,
						td => td.Query);
				}
			}

			public static IEnumerable<string> GetExceptions
			{
				get
				{
					return EnumerateTests(
						td => td.Result.StartsWith("Exception"),
						td => td.Query);
				}
			}

			static IEnumerable<T> EnumerateTests<T>(
				Func<QueryTestData, bool> predicate,
				Func<QueryTestData, T> projection)
			{
				XDocument doc = XDocument.Load(@"Hql/Ast/TestQueriesWithResults.xml");

				foreach (XElement testGroup in doc.Element("Tests").Elements("TestGroup"))
				{
					string category = testGroup.Attribute("Name").Value;

					foreach (XElement test in testGroup.Elements("Test"))
					{
						QueryTestData testData = new QueryTestData(category, test);

						if (predicate(testData))
						{
							yield return projection(testData);
						}
					}
				}
			}

			class QueryTestData
			{
				internal QueryTestData(string category, XElement xml)
				{
					Category = category;
					Query = xml.Element("Query").Value.Trim();
					Result = xml.Element("Result")?.Value;
					Name = xml.Element("Name")?.Value;
					Description = xml.Element("Description")?.Value.Trim() ?? string.Empty;
					Ignore = bool.Parse(xml.Attribute("Ignore")?.Value ?? "false");
				}

				internal string Category;
				internal string Query;
				internal string Result;
				internal string Name;
				internal string Description;
				internal bool Ignore;
			}
		}
	}
}
