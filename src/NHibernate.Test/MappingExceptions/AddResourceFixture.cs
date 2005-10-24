using System;

using NHibernate.Cfg;

using NUnit.Framework;

namespace NHibernate.Test.MappingExceptions
{
	/// <summary>
	/// Summary description for AddResourceFixture.
	/// </summary>
	[TestFixture]
	public class AddResourceFixture
	{
		[Test]
		public void ResourceNotFound() 
		{
			// add a resource that doesn't exist
			string resource = "NHibernate.Test.MappingExceptions.A.DoesNotExists.hbm.xml";
			Configuration cfg = new Configuration();
			try 
			{
				cfg.AddResource( resource, this.GetType().Assembly );
			}
			catch( MappingException me ) 
			{
				Assert.AreEqual( "Resource: " + resource + " not found", me.Message );
			}
		}

		[Test]
		public void AddDuplicateImport() 
		{
			string resource = "NHibernate.Test.MappingExceptions.A.Valid.hbm.xml";
			Configuration cfg = new Configuration();
			try 
			{
				// adding an import of "A" and then adding a class that has a 
				// name of "A" will cause a duplicate import problem.  This can
				// occur if two classes are in different namespaces but named the
				// same and the auto-import attribute is not set to "false" for one
				// of them.
				cfg.Imports["A"] = "Some other class";
				cfg.AddResource( resource, this.GetType().Assembly );
			}
			catch( MappingException me ) 
			{
				Assert.IsTrue( me.InnerException is MappingException );
				Assert.AreEqual( "duplicate import: A", me.InnerException.Message );
			}
		}
	}
}
