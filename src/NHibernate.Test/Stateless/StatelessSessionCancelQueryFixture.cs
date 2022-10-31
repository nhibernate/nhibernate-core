using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.Stateless
{
	[TestFixture]
	public class StatelessSessionCancelQueryFixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override string[] Mappings
		{
			get { return new[] { "Stateless.Document.hbm.xml" }; }
		}

		private const string _documentName = "SomeDocument";
		private CultureInfo _backupCulture;
		private CultureInfo _backupUICulture;

		protected override void OnSetUp()
		{
			if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName != CultureInfo.InvariantCulture.TwoLetterISOLanguageName)
			{
				// This test needs to run in English
				_backupCulture = CultureInfo.CurrentCulture;
				_backupUICulture = CultureInfo.CurrentUICulture;
				CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
				CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;
			}

			using (var s = Sfi.OpenStatelessSession())
			using (var t = s.BeginTransaction())
			{
				s.Insert(new Document("Some text", _documentName));
				t.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var s = Sfi.OpenStatelessSession())
			using (var t = s.BeginTransaction())
			{
				s.CreateQuery("delete Document").ExecuteUpdate();
				t.Commit();
			}

			if (_backupCulture != null)
			{
				CultureInfo.CurrentCulture = _backupCulture;
				CultureInfo.CurrentUICulture = _backupUICulture;
			}
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return TestDialect.SupportsCancelQuery &&
				TestDialect.SupportsSelectForUpdate;
		}

		private void CancelQueryTest(Action<IStatelessSession> queryAction)
		{
			using (var s1 = Sfi.OpenStatelessSession())
			using (var t1 = s1.BeginTransaction())
			{
				s1.Get<Document>(_documentName, LockMode.Upgrade);

				using (var s2 = Sfi.OpenStatelessSession())
				using (var t2 = s2.BeginTransaction())
				{
					var queryTask = Task.Factory.StartNew(() => queryAction(s2));

					Thread.Sleep(200);
					s2.CancelQuery();
					Assert.That(() => queryTask,
						Throws.InnerException.TypeOf(typeof(OperationCanceledException))
							.Or.InnerException.Message.Contains("cancel"));
				}
			}
		}

		[Test]
		public void CancelHqlQuery()
		{
			CancelQueryTest(s => s.CreateQuery("from Document d").SetLockMode("d", LockMode.Upgrade).List<Document>());
		}

		[Test]
		public void CancelLinqQuery()
		{
			CancelQueryTest(s => s.Query<Document>().WithLock(LockMode.Upgrade).ToList());
		}

		[Test]
		public void CancelQueryOverQuery()
		{
			CancelQueryTest(s => s.QueryOver<Document>().Lock().Upgrade.List());
		}
	}
}
