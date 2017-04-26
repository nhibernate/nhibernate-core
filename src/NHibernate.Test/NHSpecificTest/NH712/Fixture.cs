using System;
using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH712
{
	//<summary>Improve returned error message when default constructor is not present on a class mapped as a component</summary> 
	//<type id="4">Improvement</type> 
	//<priority id="4">Minor</priority> 
	//<created>Fri, 15 Sep 2006 07:31:13 -0400 (EDT)</created> 
	//<version>1.2.0.Alpha1</version> 
	//<component>Core</component> 
  //<link>http://nhibernate.jira.com/browse/NH-712</link> 
	[TestFixture]
	public class Fixture
	{
		[Test]
		public void Bug()
		{
			if (!Cfg.Environment.UseReflectionOptimizer)
			{
				Assert.Ignore("Test only works with reflection optimization enabled");
			}
			else
				Assert.Throws<InstantiationException>(
					() =>
					new Configuration().AddResource(GetType().Namespace + ".Mappings.hbm.xml", GetType().Assembly).BuildSessionFactory());
		}
	}
}
