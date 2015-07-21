using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.ListsWithHoles
{
    public class Employee
    {
        private int id;
        private IList<Employee> children = new List<Employee>();

        public IList<Employee> Children
        {
            get { return children; }
            set { children = value; }
        }

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

       

    }
}
