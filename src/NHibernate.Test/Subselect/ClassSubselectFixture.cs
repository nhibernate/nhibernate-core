using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.Subselect
{
	public class ClassSubselectFixture: TestCase
	{
		protected override IList Mappings
		{
			get { return new[] {"Subselect.Beings.hbm.xml"}; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		[Test]
		public void EntitySubselect()
		{
			var s = OpenSession();
			var t = s.BeginTransaction();
			Human gavin = new Human();
			gavin.Name = "gavin";
			gavin.Sex = 'M';
			gavin.Address = "Melbourne, Australia";
			Alien x23y4 = new Alien();
			x23y4.Identity = "x23y4$$hu%3";
			x23y4.Planet = "Mars";
			x23y4.Species = "martian";
			s.Save(gavin);
			s.Save(x23y4);
			s.Flush();
			var beings = s.CreateQuery("from Being").List<Being>();
			Assert.That(beings, Has.Count.GreaterThan(0));
			foreach (var being in beings)
			{
				Assert.That(being.Location, Is.Not.Null.And.Not.Empty);
				Assert.That(being.Identity, Is.Not.Null.And.Not.Empty);
				Assert.That(being.Species, Is.Not.Null.And.Not.Empty);
			}
			s.Clear();
			Sfi.Evict(typeof (Being));
			Being gav = s.Get<Being>(gavin.Id);
			Assert.That(gav.Location, Is.Not.Null.And.Not.Empty);
			Assert.That(gav.Identity, Is.Not.Null.And.Not.Empty);
			Assert.That(gav.Species, Is.Not.Null.And.Not.Empty);
			s.Clear();
			//test the <synchronized> tag:
			gavin = s.Get<Human>(gavin.Id);
			gavin.Address = "Atlanta, GA";
			gav = s.CreateQuery("from Being b where b.Location like '%GA%'").UniqueResult<Being>();
			Assert.That(gav.Location, Is.EqualTo(gavin.Address));
			s.Delete(gavin);
			s.Delete(x23y4);
			Assert.That(s.CreateQuery("from Being").List<Being>(), Is.Empty);
			t.Commit();
			s.Close();
		}
	}
}