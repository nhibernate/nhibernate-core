using System;

using NUnit.Framework;

namespace NHibernate.Test.GeneratedTest
{
	public abstract class AbstractGeneratedPropertyTest : TestCase
	{
		protected override string  MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		[Test]
		public void GeneratedProperty()
		{
			GeneratedPropertyEntity entity = new GeneratedPropertyEntity();
			entity.Name = "entity-1";
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Save(entity);
			s.Flush();
			Assert.IsNotNull(entity.LastModified, "no timestamp retrieved");
			t.Commit();
			s.Close();

			byte[] bytes = entity.LastModified;

			s = OpenSession();
			t = s.BeginTransaction();
			entity = (GeneratedPropertyEntity) s.Get(typeof(GeneratedPropertyEntity), entity.Id);
			Assert.IsTrue(NHibernateUtil.Binary.IsEqual(bytes, entity.LastModified));
			t.Commit();
			s.Close();

			Assert.IsTrue(NHibernateUtil.Binary.IsEqual(bytes, entity.LastModified));

			s = OpenSession();
			t = s.BeginTransaction();
			s.Delete(entity);
			t.Commit();
			s.Close();
		}
	}
}
