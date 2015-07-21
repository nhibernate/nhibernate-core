using System;
using NHibernate.Driver;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3620 {
    [TestFixture]
    public class Fixture : BugTestCase {
        protected override bool AppliesTo(Engine.ISessionFactoryImplementor factory) {
            return (factory.ConnectionProvider.Driver is OracleManagedDataClientDriver);
        }

        protected override void OnTearDown() {
            CleanupData();
        }

        [Test]
        public void Should_insert_two_blobs_and_a_date() {
            using (ISession s = OpenSession()) {
                var blob = new Byte[1024*24];
                for (int i = 0; i < blob.Length; i++) {
                    blob[i] = 65;
                }

                using (ITransaction tx = s.BeginTransaction()) {
                    var tb = new TwoBlobs {
                        Blob1 = blob, Blob2 = blob, Id = 1, TheDate = DateTime.Now
                    };
                    s.Save(tb);
                    tx.Commit();
                }
            }
        }

        private void CleanupData() {
            using (ISession session = OpenSession()) {
                using (ITransaction tx = session.BeginTransaction()) {
                    session.Delete("from TwoBlobs");
                    tx.Commit();
                }
            }
        }
    }
}