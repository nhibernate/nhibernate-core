using System;
using System.Collections;
using System.Reflection;

using NHibernate.Cfg;

using NUnit.Framework;

namespace NHibernate.Examples.ForumQuestions.T1078029
{
	/// <summary>
	/// Summary description for UserFixture.
	/// </summary>
	[TestFixture]
	public class MemberFixture
	{
		/// <summary>
		/// Validate that the Configuration is reading a valid Xml document correctly.
		/// </summary>
		[Test]
		public void ValidateQuickStart() 
		{
			Configuration cfg = new Configuration();
			cfg.AddResource( "NHibernate.Examples.ForumQuestions.T1078029.Member.hbm.xml", Assembly.Load("NHibernate.Examples") );
			
			ISessionFactory factory = cfg.BuildSessionFactory();
			
		}
	}
}
