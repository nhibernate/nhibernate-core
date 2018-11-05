using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.Generatedkeys.Seqidentity
{
	[TestFixture]
	public class SequenceIdentityFixture : TestCase
	{
		protected override string[] Mappings
		{
			get { return new[] { "Generatedkeys.Seqidentity.MyEntity.hbm.xml" }; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect.SupportsSequences && 
			       !(dialect is Dialect.MsSql2012Dialect) && 
			       !(dialect is Dialect.HanaDialectBase); // SAP HANA does not support a syntax allowing to return the inserted id as an output parameter or a return value
		}

		[Test]
		public void SequenceIdentityGenerator()
		{
			ISession session = OpenSession();
			session.BeginTransaction();

			var e = new MyEntity{Name="entity-1"};
			session.Save(e);

			// this insert should happen immediately!
			Assert.AreEqual(1, e.Id, "id not generated through forced insertion");

			session.Delete(e);
			session.Transaction.Commit();
			session.Close();
		}
	}
}
