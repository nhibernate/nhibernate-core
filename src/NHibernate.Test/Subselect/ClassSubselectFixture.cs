using System.Collections;
using NUnit.Framework;
using SharpTestsEx;

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
			beings.Should().Have.Count.GreaterThan(0);
			foreach (var being in beings)
			{
				being.Location.Should().Not.Be.NullOrEmpty();
				being.Identity.Should().Not.Be.NullOrEmpty();
				being.Species.Should().Not.Be.NullOrEmpty();
			}
			s.Clear();
			Sfi.Evict(typeof (Being));
			Being gav = s.Get<Being>(gavin.Id);
			gav.Location.Should().Not.Be.NullOrEmpty();
			gav.Identity.Should().Not.Be.NullOrEmpty();
			gav.Species.Should().Not.Be.NullOrEmpty();
			s.Clear();
			//test the <synchronized> tag:
			gavin = s.Get<Human>(gavin.Id);
			gavin.Address = "Atlanta, GA";
			gav = s.CreateQuery("from Being b where b.Location like '%GA%'").UniqueResult<Being>();
			gav.Location.Should().Be(gavin.Address);
			s.Delete(gavin);
			s.Delete(x23y4);
			s.CreateQuery("from Being").List<Being>().Should().Be.Empty();
			t.Commit();
			s.Close();
		}
	}
}