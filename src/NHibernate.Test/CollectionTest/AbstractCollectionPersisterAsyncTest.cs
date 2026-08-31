using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Collection.Generic;
using NHibernate.Engine;
using NHibernate.Mapping.ByCode;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NSubstitute;
using NUnit.Framework;

namespace NHibernate.Test.CollectionTest
{
	[TestFixture]
	public class AbstractCollectionPersisterAsyncTest
	{
		[Test]
		public void RecreateAsyncLeavesCancelledBatchForSessionCleanup()
		{
			var mapper = new ModelMapper();
			mapper.Class<Owner>(mapping =>
			{
				mapping.Id(x => x.Id);
				mapping.Bag(
					x => x.Values,
					collection =>
					{
						collection.Key(key => key.Column("OwnerId"));
						collection.Inverse(false);
					},
					relation => relation.Element(element => element.Column("Value")));
			});

			var configuration = TestConfigurationHelper.GetDefaultConfiguration();
			configuration.AddMapping(mapper.CompileMappingForAllExplicitlyAddedEntities());
			using var factory = (ISessionFactoryImplementor) configuration.BuildSessionFactory();
			var persister = factory.GetCollectionPersister(typeof(Owner).FullName + "." + nameof(Owner.Values));

			using var cancellationTokenSource = new CancellationTokenSource();

			var command = Substitute.For<DbCommand>();
			var batcher = Substitute.For<IBatcher>();
			batcher.PrepareBatchCommandAsync(
				       Arg.Any<CommandType>(),
				       Arg.Any<SqlString>(),
				       Arg.Any<SqlType[]>(),
				       Arg.Any<CancellationToken>())
			       .Returns(async _ =>
			       {
#if NET8_0_OR_GREATER
				       await cancellationTokenSource.CancelAsync();
#else
				       cancellationTokenSource.Cancel();
				       await Task.CompletedTask;
#endif

				       return command;
			       });

			var session = Substitute.For<ISessionImplementor>();
			session.Batcher.Returns(batcher);
			session.Factory.Returns(factory);

			var collection = new PersistentGenericBag<string>(session, new List<string> { "value" });

			Assert.That(
				async () => await persister.RecreateAsync(collection, 1, session, cancellationTokenSource.Token),
				Throws.InstanceOf<OperationCanceledException>());

			batcher.Received(1).PrepareBatchCommandAsync(
				Arg.Any<CommandType>(),
				Arg.Any<SqlString>(),
				Arg.Any<SqlType[]>(),
				cancellationTokenSource.Token);
			batcher.DidNotReceive().AbortBatch(Arg.Any<Exception>());
			batcher.DidNotReceive().CloseCommand(command, Arg.Any<DbDataReader>());
		}

		public class Owner
		{
			public virtual int Id { get; set; }
			public virtual IList<string> Values { get; set; } = new List<string>();
		}
	}
}
