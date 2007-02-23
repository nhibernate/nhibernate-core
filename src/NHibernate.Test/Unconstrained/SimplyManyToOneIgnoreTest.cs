using System;
using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.Unconstrained
{
	// represent another possible ""normal"" use (N.B. H3 create the FK if you don't use <formula>)
	[TestFixture]
	public class SimplyManyToOneIgnoreTest : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new string[] {"Unconstrained.Simply.hbm.xml"}; }
		}

		[Test]
		public void Unconstrained()
		{
			using (ISession s = OpenSession())
			{
				SimplyB sb = new SimplyB(100);
				SimplyA sa = new SimplyA("ralph");
				sa.B = sb;
				s.Save(sb);
				s.Save(sa);
				s.Flush();
			}

			using (ISession s = OpenSession())
			{
				SimplyB sb = (SimplyB) s.Get(typeof(SimplyB), 100);
				Assert.IsNotNull(sb);
				s.Delete(sb);
				s.Flush();
				SimplyA sa = (SimplyA) s.Get(typeof(SimplyA), "ralph");
				Assert.IsNull(sa.B);
				s.Delete(sa);
				s.Flush();
			}
		}
	}
}