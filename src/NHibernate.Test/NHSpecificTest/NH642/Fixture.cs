using System;
using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH642
{
	//<summary>ArgumentNullException if no setter exists and no access strategy was specified</summary>
	//<type id="1">Bug</type>
	//<priority id="3">Major</priority>    
	//<created>Fri, 9 Jun 2006 07:34:32 -0400 (EDT)</created>
	//<component>Core</component>
	//<link>http://jira.nhibernate.org</link>
	public class MissingSetter
	{
		public virtual int ReadOnly
		{
			get { return 0; }
		}
	}

	public class MissingGetter
	{
		public virtual int WriteOnly
		{
			set { }
		}
	}

	[TestFixture]
	public class Fixture
	{
		private void DoTest(string name)
		{
			try
			{
				ISessionFactory factory =
					new Configuration().AddResource("NHibernate.Test.NHSpecificTest.NH642." + name + ".hbm.xml",
					                                typeof (Fixture).Assembly).BuildSessionFactory();
				factory.Close();
			}
			catch (MappingException me)
			{
				PropertyNotFoundException found = null;
				Exception find = me;
				while (find != null)
				{
					found = find as PropertyNotFoundException;
					find = find.InnerException;
				}
				Assert.IsNotNull(found, "The PropertyNotFoundException is not present in the Exception tree.");
			}
		}

		[Test]
		public void MissingGetter()
		{
			DoTest("MissingGetter");
		}

		[Test]
		public void MissingSetter()
		{
			DoTest("MissingSetter");
		}
	}
}