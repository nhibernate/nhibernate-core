﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH372
{
	using System.Threading.Tasks;
	using System.Threading;
	[TestFixture]
	public class FixtureAsync : BugTestCase
	{
		protected bool isDynamic;

		public override string BugNumber
		{
			get { return "NH372"; }
		}

		private async Task ComponentFieldNotInserted_GenericAsync(System.Type type, CancellationToken cancellationToken = default(CancellationToken))
		{
			int id;

			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				BaseParent p = (BaseParent) Activator.CreateInstance(type);
				p.Component.FieldNotInserted = 10;
				await (session.SaveAsync(p, cancellationToken));

				await (tx.CommitAsync(cancellationToken));

				id = p.Id;
			}

			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				BaseParent p = (BaseParent) await (session.GetAsync(type, id, cancellationToken));

				Assert.AreEqual(0, p.Component.FieldNotInserted,
				                "Field should not have been inserted.");

				await (tx.CommitAsync(cancellationToken));
			}
		}

		[Test]
		public async Task ComponentFieldNotInsertedAsync()
		{
			isDynamic = false;
			await (ComponentFieldNotInserted_GenericAsync(typeof(Parent)));
		}

		[Test]
		public async Task ComponentFieldNotInserted_DynamicAsync()
		{
			isDynamic = true;
			await (ComponentFieldNotInserted_GenericAsync(typeof(DynamicParent)));
		}

		private async Task ComponentFieldNotUpdated_GenericAsync(System.Type type, CancellationToken cancellationToken = default(CancellationToken))
		{
			int id;
			int fieldInitialValue = 10;
			int fieldNewValue = 30;

			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				BaseParent p = (BaseParent) Activator.CreateInstance(type);
				p.Component.FieldNotUpdated = fieldInitialValue;
				await (session.SaveAsync(p, cancellationToken));

				await (tx.CommitAsync(cancellationToken));

				id = p.Id;
			}

			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				BaseParent p = (BaseParent) await (session.GetAsync(type, id, cancellationToken));

				Assert.AreEqual(fieldInitialValue, p.Component.FieldNotUpdated,
				                String.Format("Field should have initial inserted value of {0}.", fieldInitialValue));

				p.Component.FieldNotUpdated = fieldNewValue;
				p.Component.NormalField = 10;

				await (tx.CommitAsync(cancellationToken));
			}

			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				BaseParent p = (BaseParent) await (session.GetAsync(type, id, cancellationToken));

				Assert.AreEqual(fieldInitialValue, p.Component.FieldNotUpdated,
				                "Field should not have been updated.");

				await (tx.CommitAsync(cancellationToken));
			}
		}

		[Test]
		public async Task ComponentFieldNotUpdatedAsync()
		{
			isDynamic = false;
			await (ComponentFieldNotUpdated_GenericAsync(typeof(Parent)));
		}

		[Test]
		public async Task ComponentFieldNotUpdated_DynamicAsync()
		{
			isDynamic = true;
			await (ComponentFieldNotUpdated_GenericAsync(typeof(DynamicParent)));
		}

		private async Task SubComponentFieldNotInserted_GenericAsync(System.Type type, CancellationToken cancellationToken = default(CancellationToken))
		{
			int id;

			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				BaseParent p = (BaseParent) Activator.CreateInstance(type);
				p.Component.SubComponent.FieldNotInserted = 10;
				await (session.SaveAsync(p, cancellationToken));

				await (tx.CommitAsync(cancellationToken));

				id = p.Id;
			}

			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				BaseParent p = (BaseParent) await (session.GetAsync(type, id, cancellationToken));

				Assert.AreEqual(0, p.Component.SubComponent.FieldNotInserted,
				                "Field should not have been inserted.");

				await (tx.CommitAsync(cancellationToken));
			}
		}

		[Test]
		public async Task SubComponentFieldNotInsertedAsync()
		{
			isDynamic = false;
			await (SubComponentFieldNotInserted_GenericAsync(typeof(Parent)));
		}

		[Test]
		public async Task SubComponentFieldNotInserted_DynamicAsync()
		{
			isDynamic = false;
			await (SubComponentFieldNotInserted_GenericAsync(typeof(DynamicParent)));
		}

		private async Task SubComponentFieldNotUpdated_GenericAsync(System.Type type, CancellationToken cancellationToken = default(CancellationToken))
		{
			int id;
			int fieldInitialValue = 10;
			int fieldNewValue = 30;

			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				BaseParent p = (BaseParent) Activator.CreateInstance(type);
				p.Component.SubComponent.FieldNotUpdated = fieldInitialValue;
				await (session.SaveAsync(p, cancellationToken));

				await (tx.CommitAsync(cancellationToken));

				id = p.Id;
			}

			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				BaseParent p = (BaseParent) await (session.GetAsync(type, id, cancellationToken));

				Assert.AreEqual(fieldInitialValue, p.Component.SubComponent.FieldNotUpdated,
				                String.Format("Field should have initial inserted value of {0}.", fieldInitialValue));

				p.Component.SubComponent.FieldNotUpdated = fieldNewValue;
				p.Component.SubComponent.NormalField = 10;

				await (tx.CommitAsync(cancellationToken));
			}

			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				BaseParent p = (BaseParent) await (session.GetAsync(type, id, cancellationToken));

				Assert.AreEqual(fieldInitialValue, p.Component.SubComponent.FieldNotUpdated,
				                "Field should not have been updated.");

				await (tx.CommitAsync(cancellationToken));
			}
		}

		[Test]
		public async Task SubComponentFieldNotUpdatedAsync()
		{
			isDynamic = false;
			await (SubComponentFieldNotUpdated_GenericAsync(typeof(Parent)));
		}

		[Test]
		public async Task SubComponentFieldNotUpdated_DynamicAsync()
		{
			isDynamic = false;
			await (SubComponentFieldNotUpdated_GenericAsync(typeof(DynamicParent)));
		}

		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				if (isDynamic)
				{
					session.Delete("from DynamicParent");
				}
				else
				{
					session.Delete("from Parent");
				}
				tx.Commit();
			}
		}
	}
}