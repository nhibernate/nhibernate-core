using System;

using NHibernate.DomainModel.NHSpecific;

using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest
{
	/// <summary>
	/// Summary description for AvalonProxyFixture.
	/// </summary>
	[TestFixture]
	public class AvalonProxyFixture : TestCase 
	{
		[SetUp]
		public void SetUp() 
		{
			ExportSchema( new string[] { "NHSpecific.AvalonProxyImpl.hbm.xml"}, true );
		}

		[Test]
		public void Proxy() 
		{
			ISession s = sessions.OpenSession();
			AvalonProxy ap = new AvalonProxyImpl();
			ap.Id = 1;
			ap.Name = "first proxy";
			s.Save( ap );
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			ap = (AvalonProxy)s.Load( typeof(AvalonProxyImpl), ap.Id ); 
			Assert.IsFalse( NHibernate.IsInitialized( ap ) );
			int id = ap.Id;
			Assert.IsFalse( NHibernate.IsInitialized( ap ), "get id should not have initialized it." );
			string name = ap.Name;
			Assert.IsTrue(  NHibernate.IsInitialized( ap ), "get name should have initialized it." );
			s.Delete( ap );
			s.Flush();
			s.Close();
		}

		[Test]
		public void ProxySerialize() 
		{
			ISession s = sessions.OpenSession();
			AvalonProxy ap = new AvalonProxyImpl();
			ap.Id = 1;
			ap.Name = "first proxy";
			s.Save( ap );
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			ap = (AvalonProxy)s.Load( typeof(AvalonProxyImpl), ap.Id ); 
			Assert.AreEqual( 1, ap.Id );
			s.Disconnect();
				
			// serialize and then deserialize the session.
			System.IO.Stream stream = new System.IO.MemoryStream();
			System.Runtime.Serialization.IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter( );
			formatter.Serialize(stream, s);
			stream.Position = 0;
			s = (ISession)formatter.Deserialize(stream);
			stream.Close();


			s.Close();
		}

		[Test]
		public void SerializeNotFoundProxy() 
		{
			ISession s = sessions.OpenSession();
			// this does not actually exists in db
			AvalonProxy notThere = (AvalonProxy)s.Load( typeof(AvalonProxyImpl), 5 ); 
			Assert.AreEqual( 5, notThere.Id );
			s.Disconnect();
				
			// serialize and then deserialize the session.
			System.IO.Stream stream = new System.IO.MemoryStream();
			System.Runtime.Serialization.IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter( );
			formatter.Serialize(stream, s);
			stream.Position = 0;
			s = (ISession)formatter.Deserialize(stream);
			stream.Close();

			Assert.IsNotNull( s.Load( typeof(AvalonProxyImpl), 5 ), "should be proxy - even though it doesn't exists in db" );			
			s.Close();
		}

	}
}
