using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Transactions;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Impl;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3583
{
	[TestFixture]
	public class AutoFlushFixture : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Entity>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");

				session.Flush();
				transaction.Commit();
			}
		}

		[Test]
		public void ShouldAutoFlushWhenInExplicitTransaction()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var e1 = new Entity { Name = "Bob" };
				session.Save(e1);

				var result = (from e in session.Query<Entity>()
							  where e.Name == "Bob"
							  select e).ToList();

				Assert.That(result.Count, Is.EqualTo(1));
			}
		}
		
		[Test]
		public void AutoFlushIfRequiredShouldReturnTrue()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var e1 = new Entity { Name = "Bob" };
				session.Save(e1);

				var implementor = session as AbstractSessionImpl;
				var spaces = new HashSet<string> { "Entity" };
				Debug.Assert(implementor != null, nameof(implementor) + " != null");
				Assert.That(session.FlushMode, Is.EqualTo(FlushMode.Auto), "Issue is only presented in FlushMode.Auto");
				Assert.That(implementor.AutoFlushIfRequired(spaces), Is.True, "AutoFlushIfRequired should have reported, that the flush has been done.");
			}
		}
		

		[Test]
		public void ShouldAutoFlushWhenInDistributedTransaction()
		{
			Assume.That(Sfi.ConnectionProvider.Driver.SupportsSystemTransactions, Is.True);
			
			using (new TransactionScope())
			using (var session = OpenSession())
			{
				var e1 = new Entity { Name = "Bob" };
				session.Save(e1);

				var result = (from e in session.Query<Entity>()
							  where e.Name == "Bob"
							  select e).ToList();

				Assert.That(result.Count, Is.EqualTo(1));
			}
		}
	}
}
