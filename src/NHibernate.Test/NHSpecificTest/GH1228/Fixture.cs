using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1228
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		public void TestThetaJoinOnAssociationInSubQuery()
		{
			using var s = OpenSession();
			var queryThatWorks = s.CreateQuery(
				@"
				SELECT ROOT FROM NHibernate.Test.NHSpecificTest.GH1228.Sheet AS ROOT
					WHERE (EXISTS (FROM NHibernate.Test.NHSpecificTest.GH1228.Shelf AS inv
									, ROOT.Folder AS ROOT_Folder
								WHERE ROOT_Folder.Shelf = inv AND inv.Id = 1
							))
						AND ROOT.Name = 'SomeName'");
			queryThatWorks.List();

			queryThatWorks = s.CreateQuery(
				@"
				SELECT ROOT FROM NHibernate.Test.NHSpecificTest.GH1228.Shelf AS ROOT
					WHERE (EXISTS (FROM NHibernate.Test.NHSpecificTest.GH1228.Sheet AS sheet
									, ROOT.Folders AS ROOT_Folder
								WHERE ROOT_Folder = sheet.Folder AND sheet.Name = 'SomeName'
							))
						AND ROOT.Id = 1");
			queryThatWorks.List();
		}

		[Test]
		public void TestAnsiJoinOnAssociationInSubQuery()
		{
			if (!TestDialect.SupportsCorrelatedColumnsInSubselectJoin)
				Assert.Ignore("Dialect doesn't support this test case");

			using var s = OpenSession();
			var queryThatCreatesWrongSQL = s.CreateQuery(
				@"
				SELECT ROOT FROM NHibernate.Test.NHSpecificTest.GH1228.Sheet AS ROOT
					WHERE (EXISTS (FROM NHibernate.Test.NHSpecificTest.GH1228.Shelf AS inv
								JOIN ROOT.Folder AS ROOT_Folder
								WHERE ROOT_Folder.Shelf = inv AND inv.Id = 1
							))
						AND ROOT.Name = 'SomeName'");
			queryThatCreatesWrongSQL.List();

			// The only assertion here is that the generated SQL is valid and can be executed.
			// With the bug, the generated SQL is missing the JOIN inside the subselect (EXISTS) to Folder.
			queryThatCreatesWrongSQL = s.CreateQuery(
				@"
				SELECT ROOT FROM NHibernate.Test.NHSpecificTest.GH1228.Shelf AS ROOT
					WHERE (EXISTS (FROM NHibernate.Test.NHSpecificTest.GH1228.Sheet AS sheet
								JOIN ROOT.Folders AS ROOT_Folder
								WHERE ROOT_Folder = sheet.Folder AND sheet.Name = 'SomeName'
							))
						AND ROOT.Id = 1");
			queryThatCreatesWrongSQL.List();
			// The only assertion here is that the generated SQL is valid and can be executed.
			// With the bug, the generated SQL is missing the JOIN inside the subselect (EXISTS) to Folder.
		}

		[Test]
		public void TestOtherAnsiJoinOnAssociationInSubQuery()
		{
			using var s = OpenSession();

			// The only assertion here is that the generated SQL is valid and can be executed.
			// With the bug, the generated SQL is missing the JOIN inside the subselect (EXISTS) to Folder.
			var queryThatCreatesWrongSQL = s.CreateQuery(
				@"
				SELECT ROOT FROM NHibernate.Test.NHSpecificTest.GH1228.Shelf AS ROOT
					WHERE (EXISTS (FROM NHibernate.Test.NHSpecificTest.GH1228.Sheet AS sheet
								JOIN sheet.Folder AS folder
								WHERE folder.Shelf = ROOT AND sheet.Name = 'SomeName'
							))
						AND ROOT.Id = 1");
			queryThatCreatesWrongSQL.List();
			
			// The only assertion here is that the generated SQL is valid and can be executed.
			// With the bug, the generated SQL is missing the JOIN inside the subselect (EXISTS) to Folder.
			queryThatCreatesWrongSQL = s.CreateQuery(
				@"
				SELECT ROOT FROM NHibernate.Test.NHSpecificTest.GH1228.Sheet AS ROOT
					WHERE (EXISTS (FROM NHibernate.Test.NHSpecificTest.GH1228.Shelf AS inv
								JOIN inv.Folders AS folder
								WHERE folder = ROOT.Folder AND inv.Id = 1
							))
						AND ROOT.Name = 'SomeName'");
			queryThatCreatesWrongSQL.List();
		}
	}
}
