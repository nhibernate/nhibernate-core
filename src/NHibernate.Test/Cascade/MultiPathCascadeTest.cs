﻿using System.Linq;
using NHibernate.Proxy;
using NUnit.Framework;

namespace NHibernate.Test.Cascade
{
	[TestFixture]
	public class MultiPathCascadeTest : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override string[] Mappings
		{
			get { return new[] { "Cascade.MultiPathCascade.hbm.xml" }; }
		}

		[Test]
		public void MultiPathMergeModifiedDetached()
		{
			// persist a simple A in the database
			A a = new A();

			using (ISession s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				a.Data = "Anna";
				s.Save(a);
				t.Commit();
			}
			// modify detached entity
			this.ModifyEntity(a);

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				a = s.Merge(a);
				t.Commit();
			}
	
			this.VerifyModifications(a.Id);
		}
		
		[Test]
		public void MultiPathMergeModifiedDetachedIntoProxy()
		{
			// persist a simple A in the database
			A a = new A();
			using (ISession s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				a.Data = "Anna";
				s.Save(a);
				t.Commit();
			}
			// modify detached entity
			this.ModifyEntity(a);

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				A aLoaded = s.Load<A>(a.Id);
				Assert.That(aLoaded, Is.InstanceOf<INHibernateProxy>());
				Assert.That(s.Merge(a), Is.SameAs(aLoaded));
				t.Commit();
			}
	
			this.VerifyModifications(a.Id);
		}
		
		[Test]
		public void MultiPathUpdateModifiedDetached()
		{
			// persist a simple A in the database
			A a = new A();

			using (ISession s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				a.Data = "Anna";
				s.Save(a);
				t.Commit();
			}
	
			// modify detached entity
			this.ModifyEntity(a);

			using (ISession s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Update(a);
				t.Commit();
			}
			this.VerifyModifications(a.Id);
		}
	
		[Test]
		public void MultiPathGetAndModify()
		{
			// persist a simple A in the database
			A a = new A();

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				a.Data = "Anna";
				s.Save(a);
				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				// retrieve the previously saved instance from the database, and update it
				a = s.Get<A>(a.Id);
				this.ModifyEntity(a);
				t.Commit();
			}
			this.VerifyModifications(a.Id);
		}
	
		[Test]
		public void MultiPathMergeNonCascadedTransientEntityInCollection()
		{
			// persist a simple A in the database
			A a = new A();

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				a.Data = "Anna";
				s.Save(a);
				t.Commit();
			}	
			// modify detached entity
			this.ModifyEntity(a);

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				a = s.Merge(a);
				t.Commit();
			}
			this.VerifyModifications(a.Id);
	
			// add a new (transient) G to collection in h
			// there is no cascade from H to the collection, so this should fail when merged
			Assert.That(a.Hs.Count, Is.EqualTo(1));
			H h = a.Hs.First();
			G gNew = new G();
			gNew.Data = "Gail";
			gNew.Hs.Add(h);
			h.Gs.Add(gNew);

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				try
				{
					s.Merge(a);
					s.Merge(h);
					Assert.Fail("should have thrown TransientObjectException");
				}
				catch (TransientObjectException)
				{
					// expected
				}
			}
		}
	
		[Test]
		public void MultiPathMergeNonCascadedTransientEntityInOneToOne()
		{
			// persist a simple A in the database
			A a = new A();

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				a.Data = "Anna";
				s.Save(a);
				t.Commit();
			}
			// modify detached entity
			this.ModifyEntity(a);

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				a = (A) s.Merge(a);
				t.Commit();
			}
			this.VerifyModifications(a.Id);
	
			// change the one-to-one association from g to be a new (transient) A
			// there is no cascade from G to A, so this should fail when merged
			G g = a.G;
			a.G = null;
			A aNew = new A();
			aNew.Data = "Alice";
			g.A = aNew;
			aNew.G = g;

			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				try
				{
					s.Merge(a);
					s.Merge(g);
					Assert.Fail("should have thrown TransientObjectException");
				}
				catch (TransientObjectException)
				{
					// expected
				}
			}
		}
	
		[Test]
		public void MultiPathMergeNonCascadedTransientEntityInManyToOne()
		{
			// persist a simple A in the database
			A a = new A();

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				a.Data = "Anna";
				s.Save(a);
				t.Commit();
			}
			// modify detached entity
			this.ModifyEntity(a);

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				a = (A) s.Merge(a);
				t.Commit();
			}
			this.VerifyModifications(a.Id);
	
			// change the many-to-one association from h to be a new (transient) A
			// there is no cascade from H to A, so this should fail when merged
			Assert.That(a.Hs.Count, Is.EqualTo(1));
			H h = a.Hs.First();
			a.Hs.Remove(h);
			A aNew = new A();
			aNew.Data = "Alice";
			aNew.AddH(h);

			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				try
				{
					s.Merge(a);
					s.Merge(h);
					Assert.Fail("should have thrown TransientObjectException");
				}
				catch (TransientObjectException)
				{
					// expected
				}
			}
		}
		
		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Delete("from H");
				session.Delete("from G");
				session.Delete("from A");
				transaction.Commit();
			}
			base.OnTearDown();
		}
		
		private void ModifyEntity(A a)
		{
			// create a *circular* graph in detached entity
			a.Data = "Anthony";
	
			G g = new G();
			g.Data = "Giovanni";
	
			H h = new H();
			h.Data = "Hellen";
	
			a.G = g;
			g.A = a;
	
			a.Hs.Add(h);
			h.A = a;
	
			g.Hs.Add(h);
			h.Gs.Add(g);
		}
	
		private void VerifyModifications(long aId)
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				// retrieve the A object and check it
				A a = s.Get<A>(aId);
				Assert.That(a.Id, Is.EqualTo(aId));
				Assert.That(a.Data, Is.EqualTo("Anthony"));
				Assert.That(a.G, Is.Not.Null);
				Assert.That(a.Hs, Is.Not.Null);
				Assert.That(a.Hs.Count, Is.EqualTo(1));

				G gFromA = a.G;
				H hFromA = a.Hs.First();

				// check the G object
				Assert.That(gFromA.Data, Is.EqualTo("Giovanni"));
				Assert.That(gFromA.A, Is.SameAs(a));
				Assert.That(gFromA.Hs, Is.Not.Null);
				Assert.That(gFromA.Hs, Is.EqualTo(a.Hs));
				Assert.That(gFromA.Hs.First(), Is.SameAs(hFromA));

				// check the H object
				Assert.That(hFromA.Data, Is.EqualTo("Hellen"));
				Assert.That(hFromA.A, Is.SameAs(a));
				Assert.That(hFromA.Gs, Is.Not.Null);
				Assert.That(hFromA.Gs.Count, Is.EqualTo(1));
				Assert.That(hFromA.Gs.First(), Is.SameAs(gFromA));

				t.Commit();
			}
		}
	}
}
