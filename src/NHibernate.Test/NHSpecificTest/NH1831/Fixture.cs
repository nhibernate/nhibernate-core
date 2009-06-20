using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1831
{
   [TestFixture]
   public class Fixture : BugTestCase
   {
      [Test]
      public void CorrectPrecedenceForBitwiseOperators()
      {
         using (var s = OpenSession())
         {
            const string hql = @"SELECT dt FROM DocumentType dt WHERE dt.SystemAction & :sysAct = :sysAct ";

            s.CreateQuery(hql).SetParameter("sysAct", SystemAction.Denunciation).List();
         }
      }
   }
}
