using NUnit.Framework;
using NHibernate.Hql.Ast.ANTLR;

namespace NHibernate.Test.HQL.Ast
{
	[TestFixture]
	public class BulkManipulation: BaseFixture
	{
		#region Non-exists

		[Test]
		public void DeleteNonExistentEntity()
		{
			using (ISession s = OpenSession())
			{
				Assert.Throws<QuerySyntaxException>(() => s.CreateQuery("delete NonExistentEntity").ExecuteUpdate());
			}
		}

		[Test]
		public void UpdateNonExistentEntity()
		{
			using (ISession s = OpenSession())
			{
				Assert.Throws<QuerySyntaxException>(() => s.CreateQuery("update NonExistentEntity e set e.someProp = ?").ExecuteUpdate());
			}
		}

		#endregion
	}
}