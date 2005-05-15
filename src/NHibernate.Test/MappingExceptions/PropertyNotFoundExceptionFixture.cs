using System;
using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.MappingExceptions
{
	[TestFixture]
	public class PropertyNotFoundExceptionFixture
	{
		[Test]
		public void MisspelledPropertyName() 
		{
			bool excCaught = false;

			// add a resource that has a bad mapping
			string resource = "NHibernate.Test.MappingExceptions.A.PropertyNotFound.hbm.xml";
			Configuration cfg = new Configuration();
			try 
			{
				cfg.AddResource( resource, this.GetType().Assembly );
				cfg.BuildSessionFactory();
			}
			catch( MappingException me ) 
			{
				//"Problem trying to set property type by reflection"
				// "Could not find a getter for property 'Naame' in class 'NHibernate.Test.MappingExceptions.A'"
				Exception inner = me.InnerException;
				Assert.IsTrue( inner.Message.IndexOf( "Naame" ) > 0, "should contain name of missing property 'Naame' in exception" );
				Assert.IsTrue( inner.Message.IndexOf( "NHibernate.Test.MappingExceptions.A" ) > 0, "should contain name of class that is missing the property" );
				excCaught = true;
			}

			Assert.IsTrue( excCaught, "Should have caught the MappingException that contains the property not found exception." );
		}
	}
}
