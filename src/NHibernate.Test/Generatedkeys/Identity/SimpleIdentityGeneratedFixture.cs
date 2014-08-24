using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.Generatedkeys.Identity
{
	[TestFixture]
	public class SimpleIdentityGeneratedFixture : TestCase
	{
		// This test is to check the support of identity generator
		// NH should choose one of the identity-style generation where the Dialect are supporting one of them
		// as identity, sequence-identity (identity.sequence), generated (identity.sequence)
		protected override IList Mappings
		{
			get { return new[] { "Generatedkeys.Identity.MyEntityIdentity.hbm.xml" }; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		[Test]
		public void SequenceIdentityGenerator()
		{
			ISession session = OpenSession();
			session.BeginTransaction();

			var e = new MyEntityIdentity { Name = "entity-1" };
			session.Save(e);

			// this insert should happen immediately!
			Assert.AreEqual(1, e.Id, "id not generated through forced insertion");

			session.Delete(e);
			session.Transaction.Commit();
			session.Close();
		}
	}
}