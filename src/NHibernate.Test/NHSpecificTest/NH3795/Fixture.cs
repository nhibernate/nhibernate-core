using System.Collections;
using NHibernate.DomainModel;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3795
{
	/// <summary>
	/// Tests in this class only failed when the code was build with the Roslyn compiler which is included in Visual Studio 2015
	/// </summary>
	[TestFixture]
	public class Fixture : TestCase
	{
		protected Child childAliasField = null;
		protected A aAliasField = null;

		protected override IList Mappings
		{
			get { return new[] {"ParentChild.hbm.xml", "ABC.hbm.xml"}; }
		}

		[Test]
		public void TestFieldAliasInQueryOver()
		{
			using (var s = sessions.OpenSession())
			{
				A rowalias = null;
				s.QueryOver(() => aAliasField)
					.SelectList(list => list
						.Select(() => aAliasField.Id).WithAlias(() => rowalias.Id))
					.List();
			}
		}

		[Test]
		public void TestFieldAliasInQueryOverWithConversion()
		{
			using (var s = sessions.OpenSession())
			{
				B rowalias = null;
				s.QueryOver(() => aAliasField)
					.SelectList(list => list
						.Select(() => ((B)aAliasField).Count).WithAlias(() => rowalias.Count))
					.List();
			}
		}

		[Test]
		public void TestFieldAliasInJoinAlias()
		{
			using (var s = sessions.OpenSession())
			{
				Child rowalias = null;
				s.QueryOver<Parent>()
					.JoinAlias(p => p.Child, () => childAliasField)
					.SelectList(list => list
						.Select(() => childAliasField.Id).WithAlias(() => rowalias.Id))
					.List();
			}
		}

		[Test]
		public void TestFieldAliasInJoinQueryOver()
		{
			using (var s = sessions.OpenSession())
			{
				Child rowalias = null;
				s.QueryOver<Parent>()
					.JoinQueryOver(p => p.Child, () => childAliasField)
					.SelectList(list => list
						.Select(() => childAliasField.Id).WithAlias(() => rowalias.Id))
				.List();
			}
		}

		[Test]
		public void TestAliasInQueryOver()
		{
			Child childAlias = null;
			A aAlias = null;
			using (var s = sessions.OpenSession())
			{
				A rowalias = null;
				s.QueryOver(() => aAlias)
					.SelectList(list => list
						.Select(() => aAlias.Id).WithAlias(() => rowalias.Id))
					.List();
			}
		}

		[Test]
		public void TestAliasInQueryOverWithConversion()
		{
			Child childAlias = null;
			A aAlias = null;
			using (var s = sessions.OpenSession())
			{
				B rowalias = null;
				s.QueryOver(() => aAlias)
					.SelectList(list => list
						.Select(() => ((B) aAlias).Count).WithAlias(() => rowalias.Count))
					.List();
			}
		}

		[Test]
		public void TestAliasInJoinAlias()
		{
			Child childAlias = null;
			A aAlias = null;
			using (var s = sessions.OpenSession())
			{
				Child rowalias = null;
				s.QueryOver<Parent>()
					.JoinAlias(p => p.Child, () => childAlias)
					.SelectList(list => list
						.Select(() => childAlias.Id).WithAlias(() => rowalias.Id))
					.List();
			}
		}

		[Test]
		public void TestAliasInJoinQueryOver()
		{
			Child childAlias = null;
			A aAlias = null;
			using (var s = sessions.OpenSession())
			{
				Child rowalias = null;
				s.QueryOver<Parent>()
					.JoinQueryOver(p => p.Child, () => childAlias)
					.SelectList(list => list
						.Select(() => childAlias.Id).WithAlias(() => rowalias.Id))
					.List();
			}
		}
	}
}
