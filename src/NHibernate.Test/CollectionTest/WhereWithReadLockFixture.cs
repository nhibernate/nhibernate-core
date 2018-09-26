using System.Reflection;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.Id;
using NHibernate.Persister.Collection;
using NHibernate.SqlCommand;
using NUnit.Framework;

namespace NHibernate.Test.CollectionTest
{
	public class DialectWithReadLockHint : GenericDialect
	{
		public const string ReadOnlyLock = " for read only";

		public override string GetForUpdateString(LockMode lockMode)
		{
			if (lockMode == LockMode.Read)
			{
				return ReadOnlyLock;
			}

			return string.Empty;
		}
	}

	// SQL Anywhere has this, but has no CI currently. So testing with an ad-hoc generic dialect.
	// Trouble originally spotted with NHSpecificTest.BagWithLazyExtraAndFilter.Fixture.CanUseFilterForLazyExtra
	// test, which was locally failing for SQL Anywhere.
	[TestFixture]
	public class WhereWithReadLockFixture
	{
		private ISessionFactoryImplementor _sessionFactory;
		private ICollectionPersister _bagPersister;
		private ICollectionPersister _mapPersister;

		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			var configuration = TestConfigurationHelper.GetDefaultConfiguration();
			configuration.AddResource(
				typeof(WhereWithReadLockFixture).Namespace + ".Domain.hbm.xml",
				typeof(WhereWithReadLockFixture).Assembly);
			configuration.SetProperty(Environment.Dialect, typeof(DialectWithReadLockHint).AssemblyQualifiedName);

			_sessionFactory = (ISessionFactoryImplementor) configuration.BuildSessionFactory();
			_bagPersister =
				_sessionFactory.GetCollectionPersister(typeof(Env).FullName + "." + nameof(Env.RequestsFailed));
			Assert.That(
				_bagPersister,
				Is.InstanceOf(typeof(AbstractCollectionPersister)),
				"Unexpected bag persister type");
			_mapPersister =
				_sessionFactory.GetCollectionPersister(typeof(Env).FullName + "." + nameof(Env.FailedRequestsById));
			Assert.That(
				_mapPersister,
				Is.InstanceOf(typeof(AbstractCollectionPersister)),
				"Unexpected map persister type");
		}

		[OneTimeTearDown]
		public void OneTimeTearDown()
		{
			_sessionFactory?.Dispose();
		}

		[Test]
		public void GenerateLockHintAtEndForExtraLazyCount()
		{
			var selectMethod = typeof(AbstractCollectionPersister).GetMethod(
				"GenerateSelectSizeString",
				BindingFlags.Instance | BindingFlags.NonPublic);
			Assert.That(selectMethod, Is.Not.Null, "Unable to find GenerateSelectSizeString method");

			using (var s = _sessionFactory.OpenSession())
			{
				var select = (SqlString) selectMethod.Invoke(_bagPersister, new object[] { s });
				Assert.That(select.ToString(), Does.EndWith(DialectWithReadLockHint.ReadOnlyLock));

				s.EnableFilter("CurrentOnly");
				select = (SqlString) selectMethod.Invoke(_bagPersister, new object[] { s });
				Assert.That(select.ToString(), Does.EndWith(DialectWithReadLockHint.ReadOnlyLock));
			}
		}

		[Test]
		public void GenerateLockHintAtEndForDetectRowByIndex()
		{
			var sqlField = typeof(AbstractCollectionPersister).GetField(
				"sqlDetectRowByIndexString",
				BindingFlags.Instance | BindingFlags.NonPublic);
			Assert.That(sqlField, Is.Not.Null, "Unable to find sqlDetectRowByIndexString field");

			var sql = (SqlString) sqlField.GetValue(_mapPersister);
			Assert.That(sql.ToString(), Does.EndWith(DialectWithReadLockHint.ReadOnlyLock));
		}

		[Test]
		public void GenerateLockHintAtEndForSelectRowByIndex()
		{
			var sqlField = typeof(AbstractCollectionPersister).GetField(
				"sqlSelectRowByIndexString",
				BindingFlags.Instance | BindingFlags.NonPublic);
			Assert.That(sqlField, Is.Not.Null, "Unable to find sqlSelectRowByIndexString field");

			var sql = (SqlString) sqlField.GetValue(_mapPersister);
			Assert.That(sql.ToString(), Does.EndWith(DialectWithReadLockHint.ReadOnlyLock));
		}

		[Test]
		public void GenerateLockHintAtEndForDetectRowByElement()
		{
			var sqlField = typeof(AbstractCollectionPersister).GetField(
				"sqlDetectRowByElementString",
				BindingFlags.Instance | BindingFlags.NonPublic);
			Assert.That(sqlField, Is.Not.Null, "Unable to find sqlDetectRowByElementString field");

			var sql = (SqlString) sqlField.GetValue(_mapPersister);
			Assert.That(sql.ToString(), Does.EndWith(DialectWithReadLockHint.ReadOnlyLock));
		}

		[Test]
		public void GenerateLockHintAtEndForSelectByUniqueKey()
		{
			var sql = ((IPostInsertIdentityPersister) _bagPersister).GetSelectByUniqueKeyString("blah");
			Assert.That(sql.ToString(), Does.EndWith(DialectWithReadLockHint.ReadOnlyLock));
		}
	}
}
