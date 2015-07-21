using System.Data;
using NHibernate.Mapping;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2302
{
    [TestFixture]
    public class Fixture : BugTestCase
    {
		protected override void Configure(Cfg.Configuration configuration)
		{
			foreach (var cls in configuration.ClassMappings)
			{
				foreach (var prop in cls.PropertyIterator)
				{
					foreach (var col in prop.ColumnIterator)
					{
						if (col is Column)
						{
							var column = col as Column;
							if (column.SqlType == "nvarchar(max)")
								column.SqlType = Dialect.GetLongestTypeName(DbType.String);
						}
					}
				}
			}
		}

        protected override void OnTearDown()
        {
            CleanUp();

            base.OnTearDown();
        }

        [Test]
        public void StringHugeLength()
        {
            int id;
            // buildup a string the exceed the mapping
            string str = GetFixedLengthString12000();

            using (ISession sess = OpenSession())
            using (ITransaction tx = sess.BeginTransaction())
            {
                // create and save the entity
                StringLengthEntity entity = new StringLengthEntity();
                entity.StringHugeLength = str;
                sess.Save(entity);
                tx.Commit();
                id = entity.ID;
            }

            using (ISession sess = OpenSession())
            using (ITransaction tx = sess.BeginTransaction())
            {
                StringLengthEntity loaded = sess.Get<StringLengthEntity>(id);
                Assert.IsNotNull(loaded);
                Assert.AreEqual(12000, loaded.StringHugeLength.Length);
                Assert.AreEqual(str, loaded.StringHugeLength);
                tx.Commit();
            }
        }

        [Test, Ignore("Not supported without specify the string length.")]
        public void StringSqlType()
        {
            int id;
            // buildup a string the exceed the mapping
            string str = GetFixedLengthString12000();

            using (ISession sess = OpenSession())
            using (ITransaction tx = sess.BeginTransaction())
            {
                // create and save the entity
                StringLengthEntity entity = new StringLengthEntity();
                entity.StringSqlType = str;
                sess.Save(entity);
                tx.Commit();
                id = entity.ID;
            }

            using (ISession sess = OpenSession())
            using (ITransaction tx = sess.BeginTransaction())
            {
                StringLengthEntity loaded = sess.Get<StringLengthEntity>(id);
                Assert.IsNotNull(loaded);
                Assert.AreEqual(12000, loaded.StringSqlType.Length);
                Assert.AreEqual(str, loaded.StringSqlType);
                tx.Commit();
            }
        }

        [Test]
        public void BlobSqlType()
        {
            int id;
            // buildup a string the exceed the mapping
            string str = GetFixedLengthString12000();

            using (ISession sess = OpenSession())
            using (ITransaction tx = sess.BeginTransaction())
            {
                // create and save the entity
                StringLengthEntity entity = new StringLengthEntity();
                entity.BlobSqlType = str;
                sess.Save(entity);
                tx.Commit();
                id = entity.ID;
            }

            using (ISession sess = OpenSession())
            using (ITransaction tx = sess.BeginTransaction())
            {
                StringLengthEntity loaded = sess.Get<StringLengthEntity>(id);
                Assert.IsNotNull(loaded);
                Assert.AreEqual(12000, loaded.BlobSqlType.Length);
                Assert.AreEqual(str, loaded.BlobSqlType);
                tx.Commit();
            }
        }

				[Test]
				public void BlobWithLength()
				{
					int id;
					// buildup a string the exceed the mapping
					string str = GetFixedLengthString12000();

					using (ISession sess = OpenSession())
					using (ITransaction tx = sess.BeginTransaction())
					{
						// create and save the entity
						StringLengthEntity entity = new StringLengthEntity();
						entity.BlobLength = str;
						sess.Save(entity);
						tx.Commit();
						id = entity.ID;
					}

					using (ISession sess = OpenSession())
					using (ITransaction tx = sess.BeginTransaction())
					{
						StringLengthEntity loaded = sess.Get<StringLengthEntity>(id);
						Assert.IsNotNull(loaded);
						Assert.AreEqual(12000, loaded.BlobLength.Length);
						Assert.AreEqual(str, loaded.BlobLength);
						tx.Commit();
					}
				}

				[Test]
				public void BlobWithoutLength()
				{
					int id;
					// buildup a string the exceed the mapping
					string str = GetFixedLengthString12000();

					using (ISession sess = OpenSession())
					using (ITransaction tx = sess.BeginTransaction())
					{
						// create and save the entity
						StringLengthEntity entity = new StringLengthEntity();
						entity.Blob = str;
						sess.Save(entity);
						tx.Commit();
						id = entity.ID;
					}

					using (ISession sess = OpenSession())
					using (ITransaction tx = sess.BeginTransaction())
					{
						StringLengthEntity loaded = sess.Get<StringLengthEntity>(id);
						Assert.IsNotNull(loaded);
						Assert.AreEqual(12000, loaded.Blob.Length);
						Assert.AreEqual(str, loaded.Blob);
						tx.Commit();
					}
				}

        private void CleanUp()
        {
            using (ISession session = OpenSession())
            using (ITransaction tx = session.BeginTransaction())
            {
                session.Delete("from StringLengthEntity");
                tx.Commit();
            }
        }

        private static string GetFixedLengthString12000()
        {
            return new string('a', 12000);
        }

    }
}
