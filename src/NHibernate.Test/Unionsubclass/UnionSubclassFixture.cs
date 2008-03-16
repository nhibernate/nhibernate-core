using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.Unionsubclass
{
	[TestFixture]
	public class UnionSubclassFixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new string[] { "Unionsubclass.Beings.hbm.xml" }; }
		}

		[Test]
		public void UnionSubclassCollection()
		{
			using (ISession s = OpenSession())
			{
				using (ITransaction t = s.BeginTransaction())
				{
					Location mel = new Location("Earth");
					s.Save(mel);

					Human gavin = new Human();
					gavin.Identity = "gavin";
					gavin.Sex = 'M';
					gavin.Location = mel;
					mel.AddBeing(gavin);

					gavin.Info.Add("bar");
					gavin.Info.Add("y");
					
					t.Commit();
				}
				s.Close();
			}

			using (ISession s = OpenSession())
			{
				using (ITransaction t = s.BeginTransaction())
				{
					Human gavin = (Human)s.CreateCriteria(typeof(Human)).UniqueResult();
					Assert.AreEqual(gavin.Info.Count, 2);
					s.Delete(gavin);
					s.Delete(gavin.Location);
					t.Commit();
				}
				s.Close();
			}
		}
	}
}