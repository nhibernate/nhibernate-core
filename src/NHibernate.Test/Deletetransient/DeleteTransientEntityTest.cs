using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.Deletetransient
{
	[TestFixture]
	public class DeleteTransientEntityTest : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new string[] {"Deletetransient.Person.hbm.xml"}; }
		}

		[Test]
		public void TransientEntityDeletionNoCascades()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Delete(new Address());
			t.Commit();
			s.Close();
		}

		[Test]
		public void TransientEntityDeletionCascadingToTransientAssociation()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Person p = new Person();
			p.Addresses.Add(new Address());
			s.Delete(p);
			t.Commit();
			s.Close();
		}

		[Test]
		public void TransientEntityDeleteCascadingToCircularity()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Person p1 = new Person();
			Person p2 = new Person();
			p1.Friends.Add(p2);
			p2.Friends.Add(p1);
			s.Delete(p1);
			t.Commit();
			s.Close();
		}

		[Test]
		public void TransientEntityDeletionCascadingToDetachedAssociation()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Address address = new Address();
			address.Info = "123 Main St.";
			s.Save(address);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			Person p = new Person();
			p.Addresses.Add(address);
			s.Delete(p);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			long count = s.CreateQuery("select count(*) from Address").List<long>()[0];
			Assert.That(count, Is.EqualTo(0L), "delete not cascaded properly across transient entity");
			t.Commit();
			s.Close();
		}

		[Test]
		public void TransientEntityDeletionCascadingToPersistentAssociation()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Address address = new Address();
			address.Info = "123 Main St.";
			s.Save(address);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			address = s.Get<Address>(address.Id);
			Person p = new Person();
			p.Addresses.Add(address);
			s.Delete(p);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			long count = s.CreateQuery("select count(*) from Address").List<long>()[0];
			Assert.That(count, Is.EqualTo(0L), "delete not cascaded properly across transient entity");
			t.Commit();
			s.Close();
		}
	}
}