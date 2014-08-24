using NUnit.Framework;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.ElementsEnums
{
	public abstract class AbstractIntEnumsBagFixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		[Test]
		[Description("Should load the list of enums (NH-1772)")]
		public void LoadEnums()
		{
			object savedId;
			using (ISession s = OpenSession())
			using (s.BeginTransaction())
			{
				savedId = s.Save(new SimpleWithEnums { Things = new List<Something> { Something.B, Something.C, Something.D, Something.E } });
				s.Transaction.Commit();
			}

			using (ISession s = OpenSession())
			using (s.BeginTransaction())
			{
				var swe = s.Get<SimpleWithEnums>(savedId);
				Assert.That(swe.Things, Is.EqualTo(new[] { Something.B, Something.C, Something.D, Something.E }));
				s.Transaction.Commit();
			}

			using (ISession s = OpenSession())
			using (s.BeginTransaction())
			{
				s.Delete("from SimpleWithEnums");
				s.Transaction.Commit();
			}
		}
	}
}