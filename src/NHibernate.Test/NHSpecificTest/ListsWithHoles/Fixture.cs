using System;
using System.Collections.Generic;
using System.Text;
using log4net.Appender;
using log4net.Core;
using log4net.Repository.Hierarchy;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.ListsWithHoles
{
    using System.Collections;

    [TestFixture]
    public class Fixture : TestCase
    {

        protected override IList Mappings
        {
            get { return new string[] { "NHSpecificTest.ListsWithHoles.Mappings.hbm.xml" }; }
        }

        protected override string MappingsAssembly
        {
            get { return "NHibernate.Test"; }
        }

        [Test]
        public void CanHandleHolesInList()
        {
            int parentId, firstChildId;
            using (ISession sess = OpenSession())
            using (ITransaction tx = sess.BeginTransaction())
            {
                Employee e = new Employee();
                e.Children.Add(new Employee());
                e.Children.Add(new Employee());
                sess.Save(e);
                tx.Commit();
                parentId = e.Id;
                firstChildId = e.Children[0].Id;
            }

            using (ISession sess = OpenSession())
            using (ITransaction tx = sess.BeginTransaction())
            {
                sess.Delete(sess.Get<Employee>(firstChildId));
                tx.Commit();
            }


            using (ISession sess = OpenSession())
            using (ITransaction tx = sess.BeginTransaction())
            {
                Employee employee = sess.Get<Employee>(parentId);
                employee.Children.Add(new Employee());
                tx.Commit();
            }



            using (ISession sess = OpenSession())
            using (ITransaction tx = sess.BeginTransaction())
            {
                sess.Delete("from Employee");
                tx.Commit();
            }
        }
    }
}
