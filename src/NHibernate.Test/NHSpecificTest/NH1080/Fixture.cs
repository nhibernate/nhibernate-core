using System;
using System.Text;
using NHibernate.Test.NHSpecificTest;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1080
{
    [TestFixture]
    public class Fixture : BugTestCase
    {
        public override string BugNumber
        {
            get { return "NH1080"; }
        }


        /* Bug occurs when an HQL query joins a OneToOne association (A.C), followed by a ManyToOne (A.B2) that 
           * returns null in the resultset.
           * 
           * This results in both associations being (incorrectly) marked as one-to-one's, due to an instance variable in
           * NHibernate.Hql.Classic.PathExpressionParser.cs (DereferenceEntity method) not being cleared. 
           * NHibernate.RegisterNonExists() then incorrectly registers the null many-to-one association as missing.
           * 
           * If a proxy for another ManyToOne association is created subsequently on the same session, where the entity key
           * is the same as the one that was previously incorrectly registered as missing, an exception is thrown:
           * 
           *  NHibernate.Test.NHSpecificTest.NH1080.Fixture.TestBug : NHibernate.UnresolvableObjectException : No row with 
           * the given identifier exists: 1, of class: NHibernate.Test.NHSpecificTest.NH1080.B
           * 
           * In this test case, the exception is triggered by having an un-initialised proxy to the registered entity returned
           * in the resultset (A.B1).
           */
        [Test]
        public void TestBug()
        {
            // A.C is a one-to-one constrained mapping to class C
            // A.B1 and A.B2 are many-to-one mappings to class B

            C c = new C();
            c.ID = 1;
            c.Value = "OneToOne";

           

          
            A a = new A();
            a.ID = 1;
            a.Value = "Parent";

            B b1 = new B();
            b1.ID = 1;
            b1.Value = "Child";

            a.B1 = b1;
            a.B2 = null;
            a.C = c;

            try
            {

                using (ISession s = Sfi.OpenSession())
                {
                    s.Save(c);
                    s.Save(b1);
                    s.Save(a);
                    s.Flush();
                    s.Clear();
                    s.Close();
                }

                using (ISession s = Sfi.OpenSession())
                {
                    /* If bug is present, throws:
                    NHibernate.Test.NHSpecificTest.NH1080.Fixture.TestBug : NHibernate.UnresolvableObjectException : No row with the given identifier exists: 1, of class: NHibernate.Test.NHSpecificTest.NH1080.B
                     */
                    A loadedA = (A) s.CreateQuery("from A a join fetch a.C left join fetch a.B2").UniqueResult();
                }
            }
            finally
            {
                using (ISession s = Sfi.OpenSession())
                {
                    
                    s.Delete(a);
                    s.Delete(b1);
                    s.Delete(c);
                    s.Flush();
                }
            }
        }
    }
}
