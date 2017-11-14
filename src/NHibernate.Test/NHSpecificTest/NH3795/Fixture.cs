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
		protected Child ChildAliasField = null;
		protected A AAliasField = null;

		protected override IList Mappings
		{
			get { return new[] {"ParentChild.hbm.xml", "ABC.hbm.xml"}; }
		}

		[Test]
		public void TestFieldAliasInQueryOver()
		{
			using (var s = Sfi.OpenSession())
			{
				A rowalias = null;
				s.QueryOver(() => AAliasField)
				 .SelectList(
					 list => list
						 .Select(() => AAliasField.Id).WithAlias(() => rowalias.Id))
				 .List();
			}
		}

		[Test]
		public void TestFieldAliasInQueryOverWithConversion()
		{
			using (var s = Sfi.OpenSession())
			{
				B rowalias = null;
				s.QueryOver(() => AAliasField)
				 .SelectList(
					 list => list
						 .Select(() => ((B) AAliasField).Count).WithAlias(() => rowalias.Count))
				 .List();
			}
		}

		[Test]
		public void TestFieldAliasInJoinAlias()
		{
			using (var s = Sfi.OpenSession())
			{
				Child rowalias = null;
				s.QueryOver<Parent>()
				 .JoinAlias(p => p.Child, () => ChildAliasField)
				 .SelectList(
					 list => list
						 .Select(() => ChildAliasField.Id).WithAlias(() => rowalias.Id))
				 .List();
			}
		}

		[Test]
		public void TestFieldAliasInJoinQueryOver()
		{
			using (var s = Sfi.OpenSession())
			{
				Child rowalias = null;
				s.QueryOver<Parent>()
				 .JoinQueryOver(p => p.Child, () => ChildAliasField)
				 .SelectList(
					 list => list
						 .Select(() => ChildAliasField.Id).WithAlias(() => rowalias.Id))
				 .List();
			}
		}

		[Test]
		public void TestAliasInQueryOver()
		{
			A aAlias = null;
			using (var s = Sfi.OpenSession())
			{
				A rowalias = null;
				s.QueryOver(() => aAlias)
				 .SelectList(
					 list => list
						 .Select(() => aAlias.Id)
						 .WithAlias(() => rowalias.Id))
				 .List();
			}
		}

		[Test]
		public void TestAliasInQueryOverWithConversion()
		{
			A aAlias = null;
			using (var s = Sfi.OpenSession())
			{
				B rowalias = null;
				s.QueryOver(() => aAlias)
				 .SelectList(
					 list => list.Select(() => ((B) aAlias).Count)
								 .WithAlias(() => rowalias.Count))
				 .List();
			}
		}

		[Test]
		public void TestAliasInJoinAlias()
		{
			Child childAlias = null;

			using (var s = Sfi.OpenSession())
			{
				Child rowalias = null;
				s.QueryOver<Parent>()
				 .JoinAlias(p => p.Child, () => childAlias)
				 .SelectList(
					 list => list.Select(() => childAlias.Id)
								 .WithAlias(() => rowalias.Id))
				 .List();
			}
		}

		[Test]
		public void TestAliasInJoinQueryOver()
		{
			Child childAlias = null;

			using (var s = Sfi.OpenSession())
			{
				Child rowalias = null;
				s.QueryOver<Parent>()
				 .JoinQueryOver(p => p.Child, () => childAlias)
				 .SelectList(
					 list => list.Select(() => childAlias.Id)
								 .WithAlias(() => rowalias.Id))
				 .List();
			}
		}
	}
}
