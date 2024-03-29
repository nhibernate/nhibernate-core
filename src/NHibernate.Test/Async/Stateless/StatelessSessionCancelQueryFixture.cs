﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


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
	public class StatelessSessionCancelQueryFixtureAsync : TestCase
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

		private async Task CancelQueryTestAsync(Action<IStatelessSession> queryAction, CancellationToken cancellationToken = default(CancellationToken))
		{
			using (var s1 = Sfi.OpenStatelessSession())
			using (var t1 = s1.BeginTransaction())
			{
				await (s1.GetAsync<Document>(_documentName, LockMode.Upgrade, cancellationToken));

				using (var s2 = Sfi.OpenStatelessSession())
				using (var t2 = s2.BeginTransaction())
				{
					var queryTask = Task.Factory.StartNew(() => queryAction(s2));

					await (Task.Delay(200, cancellationToken));
					s2.CancelQuery();
					Assert.That(() => queryTask,
						Throws.InnerException.TypeOf(typeof(OperationCanceledException))
							.Or.InnerException.Message.Contains("cancel"));
				}
			}
		}

		[Test]
		public async Task CancelHqlQueryAsync()
		{
			await (CancelQueryTestAsync(s => s.CreateQuery("from Document d").SetLockMode("d", LockMode.Upgrade).List<Document>()));
		}

		[Test]
		public async Task CancelLinqQueryAsync()
		{
			await (CancelQueryTestAsync(s => s.Query<Document>().WithLock(LockMode.Upgrade).ToList()));
		}

		[Test]
		public async Task CancelQueryOverQueryAsync()
		{
			await (CancelQueryTestAsync(s => s.QueryOver<Document>().Lock().Upgrade.List()));
		}
	}
}
