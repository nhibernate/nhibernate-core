using System;
using NUnit.Framework;

namespace NHibernate.Test {
	
	[TestFixture]
	public class SimpleTest : TestCase {

		[SetUp]
		public void SetUp() {
			ExportSchema( new string[] { "Simple.hbm.xml" } );
		}

		[Test]
		public void TestSetPropertes() {
			ISession s = sessions.OpenSession();
			ITransaction t = s.BeginTransaction();
			Simple simple = new Simple();
			simple.Name = "Simple 1";
			simple.Date = DateTime.Now;
			s.Save(simple, 10);
			IQuery q = s.CreateQuery("from s in class Simple where s.Name=:Name and s.Count=:Count");
			q.SetProperties(simple);
			// The INSERT, UPDATE amd SELECT are performed due to some hacks, 
			// see Impl\SessionImpl.cs class AdoHack
			// After loading the resultset, the code breaks at SessionImpl.InitializeEntity(...).
			// Also the transaction remains opened ;-)
			//
			Assertion.Assert( q.List()[0]==simple );
			s.Delete(simple);
			t.Commit();
			s.Close();
		}
		
	}
}
