using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2374
{
	[TestFixture]
	public class NH2374Fixture : BugTestCase
	{
		[Test]
		public void OneToOne_with_EntityMode_Map()
		{
			int id;

			using (ISession sroot = OpenSession())
			{
				using (ISession s = sroot.SessionWithOptions().Connection().OpenSession())
				{
					using (ITransaction t = s.BeginTransaction())
					{
						var parent = new Hashtable();
						var child = new Hashtable
						            	{
						            		{"Parent", parent}
						            	};

						parent["Child"] = child;

						id = (int) s.Save("Parent", parent);
						s.Flush();

						t.Commit();
					}
				}
			}

			using (ISession s = OpenSession())
			{
				using (ITransaction t = s.BeginTransaction())
				{
					var p = s.Get("Parent", id) as IDictionary;

					Assert.That(p["Child"], Is.Not.Null);

					s.Delete("Parent", p);

					t.Commit();
				}
			}
		}

		[Test]
		public void OneToOne_with_EntityMode_Poco()
		{
			int id;

			using (ISession s = OpenSession())
			{
				using (ITransaction t = s.BeginTransaction())
				{
					var parent = new Hashtable();
					var child = new Hashtable
					            	{
					            		{"Parent", parent}
					            	};

					parent["Child"] = child;

					id = (int) s.Save("Parent", parent);
					s.Flush();

					t.Commit();
				}
			}

			using (ISession s = OpenSession())
			{
				using (ITransaction t = s.BeginTransaction())
				{
					var p = s.Get("Parent", id) as IDictionary;

					Assert.That(p["Child"], Is.Not.Null);

					s.Delete("Parent", p);

					t.Commit();
				}
			}
		}
	}
}