using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHibernate.Criterion;
using NHibernate.DomainModel;
using NUnit.Framework;

namespace NHibernate.Test.GenericTest.Methods
{
	[TestFixture]
	public class Fixture : TestCase
	{
		protected override string[] Mappings
		{
			get
			{
				return new string[] { "One.hbm.xml", "Many.hbm.xml", "Simple.hbm.xml" };
			}
		}

		private One one;

		protected override void OnSetUp()
		{
			base.OnSetUp();

			// create the objects to search on		
			one = new One();
			one.X = 20;
			one.Manies = new HashSet<Many>();

			Many many1 = new Many();
			many1.X = 10;
			many1.One = one;
			one.Manies.Add( many1 );

			Many many2 = new Many();
			many2.X = 20;
			many2.One = one;
			one.Manies.Add( many2 );

			var simple = new Simple(1) {Count = 1};

			using ( ISession s = OpenSession() )
			using( ITransaction t = s.BeginTransaction() )
			{
				s.Save( one );
				s.Save( many1 );
				s.Save( many2 );
				s.Save(simple, 1);
				t.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using( ISession session = OpenSession() )
			using( ITransaction tx = session.BeginTransaction() )
			{
				session.Delete( "from Many" );
				session.Delete( "from One" );
				session.Delete("from Simple");
				tx.Commit();
			}
			base.OnTearDown();
		}

		[Test]
		public void Criteria()
		{
			using( ISession s2 = OpenSession() )
			using( ITransaction t2 = s2.BeginTransaction() )
			{
				IList<One> results2 = s2.CreateCriteria( typeof( One ) )
					.Add( Expression.Eq( "X", 20 ) )
					.List<One>();

				Assert.AreEqual( 1, results2.Count );

				One one2 = results2[ 0 ];

				Assert.IsNotNull( one2, "Unable to load object" );
				Assert.AreEqual( one.X, one2.X, "Load failed" );
			}
		}

		[Test]
		public void QueryList()
		{
			using( ISession s = OpenSession() )
			using( ITransaction t = s.BeginTransaction() )
			{
				IList<One> results = s.CreateQuery( "from One" ).List<One>();

				Assert.AreEqual( 1, results.Count );
			}
		}

		[Test]
		public void QueryEnumerable()
		{
			using( ISession s = OpenSession() )
			using( ITransaction t = s.BeginTransaction() )
			{
				IEnumerable<One> results = s.CreateQuery( "from One" ).Enumerable<One>();
				IEnumerator<One> en = results.GetEnumerator();

				Assert.IsTrue( en.MoveNext() );
				Assert.IsFalse( en.MoveNext() );
			}
		}

		[Test]
		public void AutoFlushQueryEnumerable()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				Assert.That(s.FlushMode, Is.EqualTo(FlushMode.Auto));
				var results = s.CreateQuery("from Simple").Enumerable<Simple>();

				var id = 2;
				var simple = new Simple(id) {Count = id};
				s.Save(simple, id);
				var enumerator = results.GetEnumerator();

				Assert.That(enumerator.MoveNext(), Is.True);
				Assert.That(enumerator.MoveNext(), Is.True);
				Assert.That(enumerator.MoveNext(), Is.False);
				enumerator.Dispose();

				id++;
				simple = new Simple(id) {Count = id};
				s.Save(simple, id);
				enumerator = results.GetEnumerator();

				Assert.That(enumerator.MoveNext(), Is.True);
				Assert.That(enumerator.MoveNext(), Is.True);
				Assert.That(enumerator.MoveNext(), Is.True);
				Assert.That(enumerator.MoveNext(), Is.False);
				enumerator.Dispose();

				t.Rollback();
			}
		}

		[Test]
		public async Task QueryEnumerableAsync()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var results = s.CreateQuery("from One").AsyncEnumerable<One>();
				var enumerator = results.GetAsyncEnumerator();

				Assert.That(await enumerator.MoveNextAsync(), Is.True);
				Assert.That(await enumerator.MoveNextAsync(), Is.False);
			}
		}

		[Test]
		public async Task AutoFlushQueryEnumerableAsync()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				Assert.That(s.FlushMode, Is.EqualTo(FlushMode.Auto));
				var results = s.CreateQuery("from Simple").AsyncEnumerable<Simple>();

				var id = 2;
				var simple = new Simple(id) {Count = id};
				s.Save(simple, id);
				var enumerator = results.GetAsyncEnumerator();

				Assert.That(await enumerator.MoveNextAsync(), Is.True);
				Assert.That(await enumerator.MoveNextAsync(), Is.True);
				Assert.That(await enumerator.MoveNextAsync(), Is.False);
				await enumerator.DisposeAsync();

				id++;
				simple = new Simple(id) {Count = id};
				s.Save(simple, id);
				enumerator = results.GetAsyncEnumerator();

				Assert.That(await enumerator.MoveNextAsync(), Is.True);
				Assert.That(await enumerator.MoveNextAsync(), Is.True);
				Assert.That(await enumerator.MoveNextAsync(), Is.True);
				Assert.That(await enumerator.MoveNextAsync(), Is.False);
				await enumerator.DisposeAsync();

				await t.RollbackAsync();
			}
		}

		[Test]
		public void Filter()
		{
			using( ISession s = OpenSession() )
			using( ITransaction t = s.BeginTransaction() )
			{
				One one2 = ( One ) s.CreateQuery( "from One" ).UniqueResult();
				IList<Many> results = s.CreateFilter( one2.Manies, "where X = 10" )
					.List<Many>();

				Assert.AreEqual( 1, results.Count );
				Assert.AreEqual( 10, results[ 0 ].X );
				t.Commit();
			}
		}

		[Test]
		public void FilterEnumerable()
		{
			using( ISession s = OpenSession() )
			using( ITransaction t = s.BeginTransaction() )
			{
				One one2 = ( One ) s.CreateQuery( "from One" ).UniqueResult();
				IEnumerable<Many> results = s.CreateFilter( one2.Manies, "where X = 10" )
					.Enumerable<Many>();
				IEnumerator<Many> en = results.GetEnumerator();

				Assert.IsTrue( en.MoveNext() );
				Assert.AreEqual( 10, en.Current.X );
				Assert.IsFalse( en.MoveNext() );
				t.Commit();
			}
		}

		[Test]
		public async Task FilterEnumerableAsync()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				One one2 = (One) await s.CreateQuery("from One").UniqueResultAsync();
				var results = s.CreateFilter(one2.Manies, "where X = 10").AsyncEnumerable<Many>();
				var en = results.GetAsyncEnumerator();

				Assert.That(await en.MoveNextAsync(), Is.True);
				Assert.That(en.Current.X, Is.EqualTo(10));
				Assert.That(await en.MoveNextAsync(), Is.False);
				await t.CommitAsync();
			}
		}
	}
}
