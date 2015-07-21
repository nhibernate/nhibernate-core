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
			using (ITransaction t = s.BeginTransaction())
			{
				SimplyB sb = new SimplyB(100);
				SimplyA sa = new SimplyA("ralph");
				sa.SimplyB = sb;
				s.Save(sb);
				s.Save(sa);
				t.Commit();
			}

			using (ISession s = OpenSession())
			{
				using (ITransaction t = s.BeginTransaction())
				{
					SimplyB sb = (SimplyB) s.Get(typeof(SimplyB), 100);
					Assert.IsNotNull(sb);
					s.Delete(sb);
					t.Commit();
				}

				// Have to do this in a separate transaction, otherwise ISession.Get retrieves
				// the cached version of SimplyA with its B being not null.
				using (ITransaction t = s.BeginTransaction())
				{
					SimplyA sa = (SimplyA) s.Get(typeof(SimplyA), "ralph");
					Assert.IsNull(sa.SimplyB);
					s.Delete(sa);
					t.Commit();
				}
			}
		}
	}
}