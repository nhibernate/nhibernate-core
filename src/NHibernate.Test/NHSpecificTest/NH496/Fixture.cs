using System;
using NHibernate.Bytecode;
using NUnit.Framework;
using Environment=NHibernate.Cfg.Environment;

namespace NHibernate.Test.NHSpecificTest.NH496
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		public void CRUD()
		{
			if (Environment.BytecodeProvider is NullBytecodeProvider)
			{
				Assert.Ignore("This test only runs with a non-null bytecode provider");
			}
			WronglyMappedClass obj = new WronglyMappedClass();
			obj.SomeInt = 10;

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Save(obj);
				t.Commit();
			}

			try
			{
				using (ISession s = OpenSession())
				using (ITransaction t = s.BeginTransaction())
				{
					Assert.Throws<PropertyAccessException>(() => s.Get(typeof(WronglyMappedClass), obj.Id));
					t.Commit();
				}
			}
			finally
			{
				using (ISession s = OpenSession())
				using (ITransaction t = s.BeginTransaction())
				{
					s.Delete(obj);
					t.Commit();
				}
			}
		}
	}
}