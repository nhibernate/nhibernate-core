using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1228
{
	public class Fixture : BugTestCase
	{
		[Test]
		public void TestOk()
		{
			using (ISession s = OpenSession())
			{
				using (ITransaction t = s.BeginTransaction())
				{
					try
					{
						{
							var queryThatWorks = s.CreateQuery(@"
							SELECT ROOT FROM NHibernate.Test.NHSpecificTest.GH1228.Sheet AS ROOT 
							WHERE (EXISTS (FROM NHibernate.Test.NHSpecificTest.GH1228.Shelf AS inv  
										   , ROOT.Folder AS ROOT_Folder 
										   WHERE ROOT_Folder.Shelf = inv AND inv.Id = 1
								  )	   ) 
								  AND ROOT.Name = 'SomeName'");
							queryThatWorks.List();
						}
						{
							var queryThatWorks = s.CreateQuery(@"
							SELECT ROOT FROM NHibernate.Test.NHSpecificTest.GH1228.Shelf AS ROOT 
							WHERE (EXISTS (FROM NHibernate.Test.NHSpecificTest.GH1228.Sheet AS sheet
										   , ROOT.Folders AS ROOT_Folder 
										   WHERE ROOT_Folder = sheet.Folder AND sheet.Name = 'SomeName'
								  )	   ) 
								  AND ROOT.Id = 1");
							queryThatWorks.List();
						}
					}
					finally
					{
						s.Delete("from Sheet");
						s.Delete("from Folder");
						s.Delete("from Shelf");
						t.Commit();
					}
				}
			}
		}

		[Test]
		public void TestWrongSql()
		{
			using (ISession s = OpenSession())
			{
				using (ITransaction t = s.BeginTransaction())
				{
					try
					{
						{
							var queryThatCreatesWrongSQL = s.CreateQuery(@"
							SELECT ROOT FROM NHibernate.Test.NHSpecificTest.GH1228.Sheet AS ROOT 
							WHERE (EXISTS (FROM NHibernate.Test.NHSpecificTest.GH1228.Shelf AS inv  
										   JOIN ROOT.Folder AS ROOT_Folder 
										   WHERE ROOT_Folder.Shelf = inv AND inv.Id = 1
								  )	   ) 
								  AND ROOT.Name = 'SomeName'");
							queryThatCreatesWrongSQL.List();
						}
						{
							// The only assertion here is that the generated SQL is valid and can be executed.
							// Right now, the generated SQL is missing the JOIN inside the subselect (EXISTS) to Folder.
							var queryThatCreatesWrongSQL = s.CreateQuery(@"
							SELECT ROOT FROM NHibernate.Test.NHSpecificTest.GH1228.Shelf AS ROOT 
							WHERE (EXISTS (FROM NHibernate.Test.NHSpecificTest.GH1228.Sheet AS sheet
										   JOIN ROOT.Folders AS ROOT_Folder 
										   WHERE ROOT_Folder = sheet.Folder AND sheet.Name = 'SomeName'
								  )	   ) 
								  AND ROOT.Id = 1");
							queryThatCreatesWrongSQL.List();
							// The only assertion here is that the generated SQL is valid and can be executed.
							// Right now, the generated SQL is missing the JOIN inside the subselect (EXISTS) to Folder.
						}
					}
					finally
					{
						s.Delete("from Sheet");
						s.Delete("from Folder");
						s.Delete("from Shelf");
						t.Commit();
					}
				}
			}
		}

		[Test]
		public void Test3() {
			using (ISession s = OpenSession()) {
				using (ITransaction t = s.BeginTransaction()) {
					try {
						{
							// The only assertion here is that the generated SQL is valid and can be executed.
							// Right now, the generated SQL is missing the JOIN inside the subselect (EXISTS) to Folder.
							var queryThatCreatesWrongSQL = s.CreateQuery(@"
							SELECT ROOT FROM NHibernate.Test.NHSpecificTest.GH1228.Shelf AS ROOT 
							WHERE (EXISTS (FROM NHibernate.Test.NHSpecificTest.GH1228.Sheet AS sheet
										   JOIN sheet.Folder AS folder
										   WHERE folder.Shelf = ROOT AND sheet.Name = 'SomeName'
								  )	   ) 
								  AND ROOT.Id = 1");
							queryThatCreatesWrongSQL.List();
							// The only assertion here is that the generated SQL is valid and can be executed.
							// Right now, the generated SQL is missing the JOIN inside the subselect (EXISTS) to Folder.
						}
						{
							var queryThatCreatesWrongSQL = s.CreateQuery(@"
							SELECT ROOT FROM NHibernate.Test.NHSpecificTest.GH1228.Sheet AS ROOT 
							WHERE (EXISTS (FROM NHibernate.Test.NHSpecificTest.GH1228.Shelf AS inv  
										   JOIN inv.Folders AS folder
										   WHERE folder = ROOT.Folder AND inv.Id = 1
								  )	   ) 
								  AND ROOT.Name = 'SomeName'");
							queryThatCreatesWrongSQL.List();
						}
					}
					finally {
						s.Delete("from Sheet");
						s.Delete("from Folder");
						s.Delete("from Shelf");
						t.Commit();
					}
				}
			}
		}
	}
}
