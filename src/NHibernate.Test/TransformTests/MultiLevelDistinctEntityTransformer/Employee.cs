using System.Collections.Generic;

namespace NHibernate.Test.TransformTests.MultiLevelDistinctEntityTransformer
{
    public class Employee
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public Nationality Nationality { get; set; }

        public List<Child> Children { get; set; }

        public Employee()
        {
            Children = new List<Child>();
        }
    }
}