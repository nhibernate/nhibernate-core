using System;

using NHibernate.Cfg;

using NUnit.Framework;

namespace NHibernate.Test.MappingExceptions
{
	/// <summary>
	/// Summary description for AddClassFixture.
	/// </summary>
	[TestFixture]
	public class AddClassFixture
	{
		[Test]
		public void ClassMissingMappingFile() 
		{
			Configuration cfg = new Configuration();
			try 
			{
				cfg.AddClass( typeof(A) );
			}
			catch( MappingException me ) 
			{
				Assert.AreEqual( "Resource: " + typeof(A).FullName + ".hbm.xml not found", me.Message );
			}
		}

		[Test]
		public void AddClassNotFound() 
		{
			Configuration cfg = new Configuration();
			try 
			{
				cfg.AddResource( "NHibernate.Test.MappingExceptions.A.ClassNotFound.hbm.xml", this.GetType().Assembly );
			}
			catch( MappingException me ) 
			{
				Assert.AreEqual( "persistent class " + typeof(A).FullName + " not found", me.Message );
			}
		}	
	}
}
