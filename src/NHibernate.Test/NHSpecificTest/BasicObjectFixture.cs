using System;

using NHibernate;
using NHibernate.DomainModel.NHSpecific;

using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest 
{

	/// <summary>
	/// Test mappings of <c>type="Object"</c>
	/// </summary>
	/// <remarks>
	/// Moved that mapping out of ParentChildTest because MySql has a bug with writing
	/// binary types to the database.  So any TestFixture that used <see cref="NHibernate.DomainModel.Parent"/>
	/// would fail.
	/// </remarks>
	[TestFixture]
	public class BasicObjectFixture : TestCase  
	{

		[SetUp]
		public void SetUp() 
		{
			ExportSchema( new string[] { "NHSpecific.BasicObject.hbm.xml"}, true );
		}

		/// <summary>
		/// This is the replacement for ParentChildTest.ObjectType()
		/// </summary>
		[Test]
		public void TestCRUD() 
		{
			ISession s = sessions.OpenSession();
			
			BasicObjectRef any = new BasicObjectRef();
			any.Name = "the any";
			
			BasicObject bo = new BasicObject();
			bo.Name = "the object";
			bo.Any = any;

			s.Save(any);
			s.Save(bo);
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			bo = (BasicObject)s.Load( typeof(BasicObject), bo.Id );
			Assert.IsNotNull( bo.Any , "any should not be null" );
			Assert.IsTrue( bo.Any is BasicObjectRef, "any should have been a BasicObjectRef instance" );
			
			any = (BasicObjectRef)s.Load( typeof(BasicObjectRef), any.Id );
			Assert.AreSame( any, bo.Any, "any loaded and ref by BasicObject should be the same" );

			s.Delete(any);
			s.Delete(bo);
			s.Flush();
			s.Close();
		}

	}
}
