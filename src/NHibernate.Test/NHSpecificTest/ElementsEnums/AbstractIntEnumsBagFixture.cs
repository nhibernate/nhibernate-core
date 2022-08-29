using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.ElementsEnums
{
	[TestFixture]
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
			using (var t = s.BeginTransaction())
			{
				savedId = s.Save(new SimpleWithEnums { Things = new List<Something> { Something.B, Something.C, Something.D, Something.E } });
				t.Commit();
			}

			using (ISession s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var swe = s.Get<SimpleWithEnums>(savedId);
				Assert.That(swe.Things, Is.EqualTo(new[] { Something.B, Something.C, Something.D, Something.E }));
				t.Commit();
			}

			using (ISession s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Delete("from SimpleWithEnums");
				t.Commit();
			}
		}
	}
}
