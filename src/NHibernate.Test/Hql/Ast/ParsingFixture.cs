using System;
using System.Collections.Generic;
using log4net.Config;
using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.Hql.Ast.ANTLR;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace NHibernate.Test.Hql.Ast
{
	// This test need the new NUnit
	//[TestFixture]
	//public class ParsingFixture
	//{
	//  /// <summary>
	//  /// Key test for HQL strings -> HQL AST's - takes the query and returns a string
	//  /// representation of the resultant tree
	//  /// </summary>
	//  /// <param name="query"></param>
	//  /// <returns></returns>
	//  [Test]
	//  [TestCaseSource(typeof(QueryFactoryClass), "TestCases")]
	//  public string HqlParse(string query)
	//  {
	//    // This test need the new NUnit
	//    var p = new HqlParseEngine(query, false, null);
	//    p.Parse();

	//    return " " + p.Ast.ToStringTree();
	//  }

	//  /// <summary>
	//  /// Used to test individual queries "by hand", since td.net doesn't let me run a 
	//  /// single test out of a data set
	//  /// </summary>
	//  [Test]
	//  public void ManualTest()
	//  {
	//    var p = new HqlParseEngine(@"select all s, s.Other from s in class Simple where s = :s", false, null);

	//    p.Parse();

	//    Console.WriteLine(p.Ast.ToStringTree());
	//  }		

	//  /// <summary>
	//  /// Helper "test" to display queries that are ignored
	//  /// </summary>
	//  [Test]
	//  public void ShowIgnoredQueries()
	//  {
	//    foreach (string query in QueryFactoryClass.GetIgnores)
	//    {
	//      Console.WriteLine(query);
	//    }
	//  }

	//  /// <summary>
	//  /// Helper "test" to display queries that don't parse in H3
	//  /// </summary>
	//  [Test]
	//  public void ShowExceptionQueries()
	//  {
	//    foreach (string query in QueryFactoryClass.GetExceptions)
	//    {
	//      Console.WriteLine(query);
	//    }
	//  }

	//  /// <summary>
	//  /// Goes all the way to the DB and back.  Just here until there's a better place to put it...
	//  /// </summary>
	//  [Test]
	//  public void BasicQuery()
	//  {
	//    XmlConfigurator.Configure();

	//    string input = "select o.id, li.id from NHibernate.Test.CompositeId.Order o join o.LineItems li";// join o.LineItems li";

	//    ISessionFactoryImplementor sfi = SetupSFI();

	//    ISession session = sfi.OpenSession();
	//    session.CreateQuery(input).List();
	//    /*
	//    foreach (Animal o in session.CreateQuery(input).Enumerable())
	//    {
	//      Console.WriteLine(o.Description);
	//    }*/
	//  }

	//  ISessionFactoryImplementor SetupSFI()
	//  {
	//    Configuration cfg = new Configuration();
	//    cfg.AddAssembly(this.GetType().Assembly);
	//    new SchemaExport(cfg).Create(false, true);
	//    return (ISessionFactoryImplementor)cfg.BuildSessionFactory();
	//  }

	//  /// <summary>
	//  /// Class used by Nunit 2.5 to drive the data into the HqlParse test
	//  /// </summary>
	//  public class QueryFactoryClass
	//  {
	//    public static IEnumerable<TestCaseData> TestCases
	//    {
	//      get
	//      {
	//        // TODO - need to handle Ignore better (it won't show in results...)
	//        return EnumerateTests(td => !td.Ignore && !td.Result.StartsWith("Exception"),
	//                              td => new TestCaseData(td.Query)
	//                                      .Returns(td.Result)
	//                                      .SetCategory(td.Category)
	//                                      .SetName(td.Name)
	//                                      .SetDescription(td.Description));
	//      }
	//    }

	//    public static IEnumerable<string> GetIgnores
	//    {
	//      get
	//      {
	//        return EnumerateTests(td => td.Ignore,
	//                              td => td.Query);
	//      }
	//    }

	//    public static IEnumerable<string> GetExceptions
	//    {
	//      get
	//      {
	//        return EnumerateTests(td => td.Result.StartsWith("Exception"),
	//                              td => td.Query);
	//      }
	//    }

	//    static IEnumerable<T> EnumerateTests<T>(Func<QueryTestData, bool> predicate, Func<QueryTestData , T> projection)
	//    {
	//      XDocument doc = XDocument.Load(@"HQL Parsing\TestQueriesWithResults.xml");

	//      foreach (XElement testGroup in doc.Element("Tests").Elements("TestGroup"))
	//      {
	//        string category = testGroup.Attribute("Name").Value;

	//        foreach (XElement test in testGroup.Elements("Test"))
	//        {
	//          QueryTestData testData = new QueryTestData(category, test);

	//          if (predicate(testData))
	//          {
	//            yield return projection(testData);
	//          }
	//        }
	//      }
	//    }

	//    class QueryTestData
	//    {
	//      internal QueryTestData(string category, XElement xml)
	//      {
	//        Category = category;
	//        Query = xml.Element("Query").Value;
	//        Result = xml.Element("Result") != null ? xml.Element("Result").Value : "barf";
	//        Name = xml.Element("Name") != null ? xml.Element("Name").Value : null;
	//        Description = xml.Element("Description") != null ? xml.Element("Description").Value : null;
	//        Ignore = xml.Attribute("Ignore") != null ? bool.Parse(xml.Attribute("Ignore").Value) : false;
	//      }

	//      internal string Category;
	//      internal string Query;
	//      internal string Result;
	//      internal string Name;
	//      internal string Description;
	//      internal bool Ignore;
	//    }
	//  }
	//}
}