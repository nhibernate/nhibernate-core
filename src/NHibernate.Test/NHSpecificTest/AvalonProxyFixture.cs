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

	}
}
