using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2756
{
	public class Fixture : BugTestCase
	{
		[Test]
		public void Saving_SetOfComponentsWithFormulaColumn_ShouldWork()
		{
			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				var file = new File { Filename = "MyFilename" };
				var document = new Document();
				document.Files.Add(new DocumentFile { Description = "MyDescription", File = file });

				session.Save(file);
				session.Save(document);

				tx.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				session.Delete("from Document");
				session.Delete("from File");
				tx.Commit();
			}
		}
	}
}