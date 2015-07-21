using System.Collections;
using NHibernate.Classic;
using NUnit.Framework;

namespace NHibernate.Test.Classic
{
	[TestFixture]
	public class ValidatableFixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new string[] { "Classic.Video.hbm.xml" }; }
		}

		[Test]
		public void Save()
		{
			try
			{
				using (ISession s = OpenSession())
				{
					s.Save(new Video());
					s.Flush();
				}
				Assert.Fail("Saved an invalid entity");
			}
			catch(ValidationFailure)
			{
				// Ok
			}

			Video v = new Video("Shinobi", 10, 10);
			using (ISession s = OpenSession())
			{
				s.Save(v);
				s.Delete(v);
				s.Flush();
			}
		}

		[Test]
		public void Update()
		{
			Video v = new Video("Shinobi", 10, 10);
			using (ISession s = OpenSession())
			{
				s.Save(v);
				s.Flush();
			}
			int savedId = v.Id;
			// update detached
			v.Heigth = 0;
			try
			{
				using (ISession s = OpenSession())
				{
					s.Update(v);
					s.Flush();
				}
				Assert.Fail("Updated an invalid entity");
			}
			catch (ValidationFailure)
			{
				// Ok
			}

			// update in the same session
			using (ISession s = OpenSession())
			{
				Video vu = (Video)s.Get(typeof(Video), savedId);
				vu.Width = 0;
				try
				{
					s.Update(vu);
					s.Flush();
					Assert.Fail("Updated an invalid entity");
				}
				catch (ValidationFailure)
				{
					//Ok
				}
			}

			// cleanup
			using (ISession s = OpenSession())
			{
				s.Delete(v);
				s.Flush();
			}
		}

		[Test]
		public void Merge()
		{
			Video v = new Video("Shinobi", 10, 10);
			using (ISession s = OpenSession())
			{
				s.Save(v);
				s.Flush();
			}
			v.Heigth = 0;
			try
			{
				using (ISession s = OpenSession())
				{
					s.Merge(v);
					s.Flush();
				}
				Assert.Fail("Updated an invalid entity");
			}
			catch (ValidationFailure)
			{
				// Ok
			}

			Video v1 = new Video("Shinobi", 0, 10);
			try
			{
				using (ISession s = OpenSession())
				{
					s.Merge(v1);
					s.Flush();
				}
				Assert.Fail("saved an invalid entity");
			}
			catch (ValidationFailure)
			{
				// Ok
			}


			// cleanup
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Delete("from Video");
				tx.Commit();
			}
		}

		[Test]
		public void Delete()
		{
			Video v = new Video("Shinobi", 10, 10);
			using (ISession s = OpenSession())
			{
				s.Save(v);
				s.Flush();
				// Validatable not called in deletation
				s.Delete(v);
				s.Flush();
			}
		}

	}
}
