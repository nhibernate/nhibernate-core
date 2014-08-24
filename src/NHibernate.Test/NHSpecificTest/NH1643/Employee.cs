using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH1643
{
    public class Employee
    {
        public virtual int Id { get; set; }

        public virtual string FirstName { get; set; }

        public virtual string LastName { get; set; }

        private ICollection<Department> department = new List<Department>();

        public virtual ICollection<Department> Departments
        {
            get { return department; }
            set { department = value; }
        }
    }
}
