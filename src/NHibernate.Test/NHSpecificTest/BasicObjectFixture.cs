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
	/// binary types to the database.  So any TestFixture that used <see cref="NHibernateUtil.DomainModel.Parent"/>
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
		/// This is the replacement for ParentChildTest.ObjectType() and FooBarTest.ObjectType()
		/// </summary>
		[Test]
		public void TestCRUD() 
		{
			ISession s = sessions.OpenSession();
			
			BasicObjectRef any = new BasicObjectRef();
			any.Name = "the any";
			
			IBasicObjectProxy anyProxy = new BasicObjectProxy();
			anyProxy.Name = "proxied object";

			BasicObject bo = new BasicObject();
			bo.Name = "the object";
			bo.Any = any;
			bo.AnyWithProxy = anyProxy;

			s.Save(any);
			s.Save(anyProxy);
			s.Save(bo);
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			bo = (BasicObject)s.Load( typeof(BasicObject), bo.Id );

			
			Assert.IsNotNull( bo.AnyWithProxy , "AnyWithProxy should not be null" );
			Assert.IsTrue( bo.AnyWithProxy is IBasicObjectProxy, "AnyWithProxy should have been a IBasicObjectProxy instance" );
			Assert.AreEqual( anyProxy.Id, ((IBasicObjectProxy)bo.AnyWithProxy).Id );

			Assert.IsNotNull( bo.Any , "any should not be null" );
			Assert.IsTrue( bo.Any is BasicObjectRef, "any should have been a BasicObjectRef instance" );
			Assert.AreEqual( any.Id, ((BasicObjectRef)bo.Any).Id );
			
			any = (BasicObjectRef)s.Load( typeof(BasicObjectRef), any.Id );
			Assert.AreSame( any, bo.Any, "any loaded and ref by BasicObject should be the same" );

			s.Delete(bo.Any);
			s.Delete(bo.AnyWithProxy);
			s.Delete(bo);
			s.Flush();
			s.Close();
		}

	}
}
