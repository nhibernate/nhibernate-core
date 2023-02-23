﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Linq;
using NUnit.Framework;
using NHibernate.Linq;

namespace NHibernate.Test.NHSpecificTest.GH0829
{
	using System.Threading.Tasks;
	[TestFixture]
	public class FixtureAsync : BugTestCase
	{
		protected override void OnSetUp()
		{
			using var session = OpenSession();
			using var transaction = session.BeginTransaction();

			var e1 = new Parent { Type = TestEnum.A | TestEnum.C };
			session.Save(e1);

			var e2 = new Child { Type = TestEnum.D, Parent = e1 };
			session.Save(e2);

			var e3 = new Child { Type = TestEnum.C, Parent = e1 };
			session.Save(e3);

			session.Flush();
			transaction.Commit();
		}

		[Test]
		public async Task SelectClassAsync()
		{
			using var session = OpenSession();
			using var transaction = session.BeginTransaction();

			var resultFound = await (session.Query<Parent>().Where(x => x.Type.HasFlag(TestEnum.A)).FirstOrDefaultAsync());

			var resultNotFound = await (session.Query<Parent>().Where(x => x.Type.HasFlag(TestEnum.D)).FirstOrDefaultAsync());

			Assert.That(resultFound, Is.Not.Null);
			Assert.That(resultNotFound, Is.Null);
		}

		[Test]
		public async Task SelectChildClassContainedInParentAsync()
		{
			using var session = OpenSession();
			using var transaction = session.BeginTransaction();

			var result = await (session.Query<Child>().Where(x => x.Parent.Type.HasFlag(x.Type)).FirstOrDefaultAsync());

			Assert.That(result, Is.Not.Null);
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();
			using var session = OpenSession();
			foreach (var entity in new[] { nameof(Child), nameof(Parent) })
			{
				session.Delete($"from {entity}");
				session.Flush();
			}
		}
	}
}
