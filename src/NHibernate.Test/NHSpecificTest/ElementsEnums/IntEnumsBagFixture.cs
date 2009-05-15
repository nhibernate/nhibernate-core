using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.ElementsEnums
{
	[TestFixture]
	public class IntEnumsBagFixture : TestCase
	{
		protected override IList Mappings
		{
			get { return new[] { "NHSpecificTest.ElementsEnums.SimpleWithEnums.hbm.xml" }; }
		}

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
				Assert.That(swe.Things, Is.EqualTo(new[] {Something.B, Something.C, Something.D, Something.E}));
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