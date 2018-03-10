using System;
using System.Data;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Engine;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Test.NHSpecificTest.NH1553.MsSQL
{
	/// <summary>
	/// Test fixture for NH1553, which checks update conflict detection together with snapshot isolation transaction isolation level.
	/// </summary>
	[TestFixture]
	public class SnapshotIsolationUpdateConflictTest : BugTestCase
	{
		private Person person;

		public override string BugNumber
		{
			get { return "NH1553.MsSQL"; }
		}

		private ITransaction BeginTransaction(ISession session)
		{
			return session.BeginTransaction(IsolationLevel.Snapshot);
		}

		private Person LoadPerson()
		{
			using (var session = OpenSession())
			using (var tr = BeginTransaction(session))
			{
				var p = session.Get<Person>(person.Id);
				tr.Commit();
				return p;
			}
		}

		private void SavePerson(Person p)
		{
			using (var session = OpenSession())
			using (var tr = BeginTransaction(session))
			{
				session.SaveOrUpdate(p);
				session.Flush();
				tr.Commit();
			}
		}

		/// <summary>
		/// Tests, that NHibernate detects the update conflict and returns a StaleObjectStateException as expected.
		/// This behaviour is part of the standard NHibernate feature set.
		/// </summary>
		[Test]
		public void UpdateConflictDetectedByNH()
		{
			Person p1 = LoadPerson();
			Person p2 = LoadPerson();

			p1.IdentificationNumber++;
			p2.IdentificationNumber += 2;

			SavePerson(p1);
			Assert.That(p1.Version, Is.EqualTo(person.Version + 1));

			var expectedException = Sfi.Settings.IsBatchVersionedDataEnabled
				? (IResolveConstraint) Throws.InstanceOf<StaleStateException>()
				: Throws.InstanceOf<StaleObjectStateException>()
				        .And.Property("EntityName").EqualTo(typeof(Person).FullName)
				        .And.Property("Identifier").EqualTo(p2.Id);

			Assert.That(() => SavePerson(p2), expectedException);
		}

		/// <summary>
		/// Tests, that the extension provided wraps the returned SQL Exception inside a StaleObjectStateException,
		/// if the SQL Server detects an update conflict in snapshot isolation.
		/// </summary>
		[Test]
		public void UpdateConflictDetectedBySQLServer()
		{
			Person p1 = LoadPerson();

			p1.IdentificationNumber++;

			using (var session1 = OpenSession())
			using (var tr1 = BeginTransaction(session1))
			{
				session1.SaveOrUpdate(p1);
				session1.Flush();

				using (var session2 = OpenSession())
				using (var tr2 = BeginTransaction(session2))
				{
					var p2 = session2.Get<Person>(person.Id);
					p2.IdentificationNumber += 2;

					tr1.Commit();
					Assert.That(p1.Version, Is.EqualTo(person.Version + 1));

					session2.SaveOrUpdate(p2);

					var expectedException = Sfi.Settings.IsBatchVersionedDataEnabled
						? (IConstraint) Throws.InstanceOf<StaleStateException>()
						: Throws.InstanceOf<StaleObjectStateException>()
						        .And.Property("EntityName").EqualTo(typeof(Person).FullName)
						        .And.Property("Identifier").EqualTo(p2.Id);

					Assert.That(
						() =>
						{
							session2.Flush();
							tr2.Commit();
						},
						expectedException);
				}
			}
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is MsSql2005Dialect;
		}

		protected override bool AppliesTo(ISessionFactoryImplementor factory)
		{
			// SQLUpdateConflictToStaleStateExceptionConverter is specific to Sql client driver, and does not work
			// with Odbc (and likeley Oledb).
			return factory.ConnectionProvider.Driver is SqlClientDriver;
		}

		private bool _isSnapshotIsolationAlreadyAllowed;

		private void CheckAllowSnapshotIsolation()
		{
			using (var session = OpenSession())
			using (var command = session.Connection.CreateCommand())
			{
				command.CommandText = $@"select snapshot_isolation_state_desc from sys.databases 
	where name = '{session.Connection.Database}'";
				_isSnapshotIsolationAlreadyAllowed =
					StringComparer.OrdinalIgnoreCase.Equals(command.ExecuteScalar() as string, "on");
			}
		}

		private void SetAllowSnapshotIsolation(bool on)
		{
			using (var session = OpenSession())
			using (var command = session.Connection.CreateCommand())
			{
				command.CommandText = "ALTER DATABASE " + session.Connection.Database + " set allow_snapshot_isolation "
					+ (on ? "on" : "off");
				command.ExecuteNonQuery();
			}
		}

		protected override void OnSetUp()
		{
			base.OnSetUp();

			CheckAllowSnapshotIsolation();
			if (!_isSnapshotIsolationAlreadyAllowed)
				SetAllowSnapshotIsolation(true);

			person = new Person();
			person.IdentificationNumber = 123;
			SavePerson(person);
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var tr = session.BeginTransaction(IsolationLevel.Serializable))
			{
				session.Delete("from Person");
				tr.Commit();
			}

			if (!_isSnapshotIsolationAlreadyAllowed)
				SetAllowSnapshotIsolation(false);

			base.OnTearDown();
		}

		protected override void Configure(Configuration configuration)
		{
			base.Configure(configuration);
			configuration.SetProperty(Environment.SqlExceptionConverter,
			                          typeof (SQLUpdateConflictToStaleStateExceptionConverter).AssemblyQualifiedName);
		}
	}
}
