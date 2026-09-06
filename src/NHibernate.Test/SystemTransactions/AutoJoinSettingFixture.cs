using System.Transactions;
using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.SystemTransactions
{
	[TestFixture(true)]
	[TestFixture(false)]
	[TestFixture(new object[] { null })]
	public class AutoJoinSettingFixture : TestCase
	{
		private readonly bool? _autoJoinTransaction;

		public AutoJoinSettingFixture(bool? autoJoinTransaction)
		{
			_autoJoinTransaction = autoJoinTransaction;
		}

		protected override string[] Mappings => new[] { "TransactionTest.Person.hbm.xml" };

		protected override string MappingsAssembly => "NHibernate.Test";

		protected override void Configure(Configuration configuration)
		{
			if (_autoJoinTransaction.HasValue)
				configuration.SetProperty(Environment.AutoJoinTransaction, _autoJoinTransaction.ToString());
			else
				configuration.Properties.Remove(Environment.AutoJoinTransaction);
		}

		[Test]
		public void CheckTransactionJoined()
		{
			using (new TransactionScope())
			using (var s = OpenSession())
			{
				Assert.That(
					s.GetSessionImplementation().TransactionContext,
					_autoJoinTransaction == false ? Is.Null : Is.Not.Null);
			}
		}

		[Theory]
		public void CanOverrideAutoJoin(bool autoJoin)
		{
			using (new TransactionScope())
			using (var s = Sfi.WithOptions().AutoJoinTransaction(autoJoin).OpenSession())
			{
				Assert.That(
					s.GetSessionImplementation().TransactionContext,
					autoJoin ? Is.Not.Null : Is.Null);
			}
		}

		// Adapted from Hazzik bug report, #3782.
		[Theory]
		public void AutoJoinTransactionSurvivesSerialization(bool? autoJoin)
		{
			var sb = Sfi.WithOptions();
			if (autoJoin.HasValue)
				sb.AutoJoinTransaction(autoJoin.Value);

			using var session = sb.OpenSession();
			var before = ((ISessionImplementor) session).ConnectionManager.ShouldAutoJoinTransaction;

			var deserialized = (ISession) SerializationHelper.Deserialize(SerializationHelper.Serialize(session));
			var after = ((ISessionImplementor) deserialized).ConnectionManager.ShouldAutoJoinTransaction;

			Assert.That(after, Is.EqualTo(before));
		}
	}
}
