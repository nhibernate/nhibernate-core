using System;
using System.Data;
using System.Data.Common;
using NHibernate.Cfg;
using NHibernate.Mapping.ByCode;
using NHibernate.Transaction;
using NUnit.Framework;

namespace NHibernate.Test.TransactionTest
{
	[TestFixture]
	public class TransactionNotificationFixture : TestCase
	{
		public class Entity
		{
			public virtual int Id { get; set; }
			public virtual string Name { get; set; }
		}

		protected override string[] Mappings => null;

		protected override void AddMappings(Configuration configuration)
		{
			var modelMapper = new ModelMapper();
			modelMapper.Class<Entity>(
				x =>
				{
					x.Id(e => e.Id);
					x.Property(e => e.Name);
					x.Table(nameof(Entity));
				});

			configuration.AddMapping(modelMapper.CompileMappingForAllExplicitlyAddedEntities());
		}

		[Test, Obsolete]
		public void NoTransaction()
		{
			var interceptor = new RecordingInterceptor();
			var synchronisation = new Synchronization();
			using (var session = Sfi.WithOptions().Interceptor(interceptor).OpenSession())
			{
				session.Transaction.RegisterSynchronization(synchronisation);
				Assert.That(interceptor.afterTransactionBeginCalled, Is.EqualTo(0), "interceptor begin");
				Assert.That(interceptor.beforeTransactionCompletionCalled, Is.EqualTo(0), "interceptor before");
				Assert.That(interceptor.afterTransactionCompletionCalled, Is.EqualTo(0), "interceptor after");
			}
			Assert.That(synchronisation.BeforeExecutions, Is.EqualTo(0), "sync before");
			Assert.That(synchronisation.AfterExecutions, Is.EqualTo(0), "sync after");
		}

		[Test]
		public void AfterBegin()
		{
			var interceptor = new RecordingInterceptor();
			using (var session = Sfi.WithOptions().Interceptor(interceptor).OpenSession())
			using (var t = session.BeginTransaction())
			{
				var synchronisation = new Synchronization();
				t.RegisterSynchronization(synchronisation);

				Assert.That(interceptor.afterTransactionBeginCalled, Is.EqualTo(1), "interceptor begin");
				Assert.That(interceptor.beforeTransactionCompletionCalled, Is.EqualTo(0), "interceptor before");
				Assert.That(interceptor.afterTransactionCompletionCalled, Is.EqualTo(0), "interceptor after");
				Assert.That(synchronisation.BeforeExecutions, Is.EqualTo(0), "sync before");
				Assert.That(synchronisation.AfterExecutions, Is.EqualTo(0), "sync after");
			}
		}

		[Test]
		public void Commit()
		{
			var interceptor = new RecordingInterceptor();
			using (var session = Sfi.WithOptions().Interceptor(interceptor).OpenSession())
			using (var tx = session.BeginTransaction())
			{
				var synchronisation = new Synchronization();
				tx.RegisterSynchronization(synchronisation);
				tx.Commit();
				Assert.That(interceptor.afterTransactionBeginCalled, Is.EqualTo(1), "interceptor begin");
				Assert.That(interceptor.beforeTransactionCompletionCalled, Is.EqualTo(1), "interceptor before");
				Assert.That(interceptor.afterTransactionCompletionCalled, Is.EqualTo(1), "interceptor after");
				Assert.That(synchronisation.BeforeExecutions, Is.EqualTo(1), "sync before");
				Assert.That(synchronisation.AfterExecutions, Is.EqualTo(1), "sync after");
			}
		}

		[Test]
		public void Rollback()
		{
			var interceptor = new RecordingInterceptor();
			using (var session = Sfi.WithOptions().Interceptor(interceptor).OpenSession())
			using (var tx = session.BeginTransaction())
			{
				var synchronisation = new Synchronization();
				tx.RegisterSynchronization(synchronisation);
				tx.Rollback();
				Assert.That(interceptor.afterTransactionBeginCalled, Is.EqualTo(1), "interceptor begin");
				Assert.That(interceptor.beforeTransactionCompletionCalled, Is.EqualTo(0), "interceptor before");
				Assert.That(interceptor.afterTransactionCompletionCalled, Is.EqualTo(1), "interceptor after");
				Assert.That(synchronisation.BeforeExecutions, Is.EqualTo(0), "sync before");
				Assert.That(synchronisation.AfterExecutions, Is.EqualTo(1), "sync after");
			}
		}

		[Theory]
		[Description("NH2128")]
		public void ShouldNotifyAfterTransaction(bool usePrematureClose)
		{
			var interceptor = new RecordingInterceptor();
			var synchronisation = new Synchronization();
			ISession s;

			using (s = OpenSession(interceptor))
			using (var t = s.BeginTransaction())
			{
				t.RegisterSynchronization(synchronisation);
				s.CreateCriteria<object>().List();

				// Call session close while still inside transaction?
				if (usePrematureClose)
					s.Close();
			}

			Assert.That(s.IsOpen, Is.False);
			Assert.That(interceptor.afterTransactionCompletionCalled, Is.EqualTo(1), "interceptor");
			Assert.That(synchronisation.AfterExecutions, Is.EqualTo(1), "sync");
		}

		[Description("NH2128")]
		[Theory]
		public void ShouldNotifyAfterTransactionWithOwnConnection(bool usePrematureClose)
		{
			var interceptor = new RecordingInterceptor();
			var synchronisation = new Synchronization();
			ISession s;

			using (var ownConnection = Sfi.ConnectionProvider.GetConnection())
			{
				using (s = Sfi.WithOptions().Connection(ownConnection).Interceptor(interceptor).OpenSession())
				using (var t = s.BeginTransaction())
				{
					t.RegisterSynchronization(synchronisation);
					s.CreateCriteria<object>().List();

					// Call session close while still inside transaction?
					if (usePrematureClose)
						s.Close();
				}
			}

			Assert.That(s.IsOpen, Is.False);
			Assert.That(interceptor.afterTransactionCompletionCalled, Is.EqualTo(1), "interceptor");
			Assert.That(synchronisation.AfterExecutions, Is.EqualTo(1), "sync");
		}

		[Test]
		public void CanRegisterSynchronizationInCustomTransaction()
		{
			using (var custom = new CustomTransaction())
			{
				TransactionExtensions.RegisterSynchronization(custom, new Synchronization());
				Assert.That(custom.HasRegisteredSynchronization, Is.True);
			}
		}
	}

	#region Synchronization classes

	public partial class CustomTransaction : ITransaction
	{
		public bool HasRegisteredSynchronization { get; private set; }

		public void Dispose()
		{
		}

		public void Begin()
		{
			throw new NotImplementedException();
		}

		public void Begin(IsolationLevel isolationLevel)
		{
			throw new NotImplementedException();
		}

		public void Commit()
		{
			throw new NotImplementedException();
		}

		public void Rollback()
		{
			throw new NotImplementedException();
		}

		public bool IsActive => throw new NotImplementedException();
		public bool WasRolledBack => throw new NotImplementedException();
		public bool WasCommitted => throw new NotImplementedException();

		public void Enlist(DbCommand command)
		{
			throw new NotImplementedException();
		}

		[Obsolete]
		public void RegisterSynchronization(ISynchronization synchronization)
		{
			throw new NotImplementedException();
		}

		public void RegisterSynchronization(ITransactionCompletionSynchronization synchronization)
		{
			if (synchronization == null)
				throw new ArgumentNullException(nameof(synchronization));
			HasRegisteredSynchronization = true;
		}
	}

	public partial class Synchronization : ITransactionCompletionSynchronization
	{
		public int BeforeExecutions { get; private set; }
		public int AfterExecutions { get; private set; }

		public void ExecuteBeforeTransactionCompletion()
		{
			BeforeExecutions += 1;
		}

		public void ExecuteAfterTransactionCompletion(bool success)
		{
			AfterExecutions += 1;
		}
	}

	#endregion
}
