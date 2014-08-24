using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2773 {
   public class Fixture : BugTestCase {
      private Guid _entityGuid;

      protected override void OnSetUp() {
         using (ISession session = OpenSession()) {
            using (ITransaction tx = session.BeginTransaction()) {
               var entity = new MyEntity();
               var entity2 = new MyEntity();
               entity.OtherEntity = entity2;

               session.Save(entity);
               session.Save(entity2);

               _entityGuid = entity.Id;

               tx.Commit();
            }
         }
      }

      protected override void OnTearDown() {
         base.OnTearDown();

         using (ISession session = OpenSession()) {
            using (ITransaction tx = session.BeginTransaction()) {
               session.Delete("from MyEntity");
               tx.Commit();
            }
         }
      }

      [Test]
      public void DeserializedSession_ProxyType_ShouldBeEqualToOriginalProxyType() {
         System.Type originalProxyType = null;
         System.Type deserializedProxyType = null;
         ISession deserializedSession = null;

         using (ISession session = OpenSession()) {
            using (ITransaction tx = session.BeginTransaction()) {
               var entity = session.Get<MyEntity>(_entityGuid);
               originalProxyType = entity.OtherEntity.GetType();
               tx.Commit();
            }


            using (MemoryStream sessionMemoryStream = new MemoryStream()) {
               BinaryFormatter formatter = new BinaryFormatter();
               formatter.Serialize(sessionMemoryStream, session);

               sessionMemoryStream.Seek(0, SeekOrigin.Begin);
               deserializedSession = (ISession)formatter.Deserialize(sessionMemoryStream);
            }
         }

         using (ITransaction tx = deserializedSession.BeginTransaction()) {
            var entity = deserializedSession.Get<MyEntity>(_entityGuid);
            deserializedProxyType = entity.OtherEntity.GetType();
            tx.Commit();
         }

         deserializedSession.Dispose();

         Assert.AreEqual(originalProxyType, deserializedProxyType);
      }
   }
}